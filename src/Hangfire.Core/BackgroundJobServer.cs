// This file is part of Hangfire.
// Copyright © 2013-2014 Sergey Odinokov.
// 
// Hangfire is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as 
// published by the Free Software Foundation, either version 3 
// of the License, or any later version.
// 
// Hangfire is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public 
// License along with Hangfire. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire.Annotations;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Logging;
using Hangfire.Server;
using Hangfire.States;
using System.Configuration;

namespace Hangfire
{
    public class BackgroundJobServer : IDisposable, IObserver<int>
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly BackgroundJobServerOptions _options;
        private readonly IDisposable _processingServer;        
        private DateTime _lastEmailNotification;
        private bool _hasFirstFailedDateTime;
        private long _failedJobsCount;
        private object _failedLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundJobServer"/> class
        /// with default options and <see cref="JobStorage.Current"/> storage.
        /// </summary>
        public BackgroundJobServer()
            : this(new BackgroundJobServerOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundJobServer"/> class
        /// with default options and the given storage.
        /// </summary>
        /// <param name="storage">The storage</param>
        public BackgroundJobServer([NotNull] JobStorage storage)
            : this(new BackgroundJobServerOptions(), storage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundJobServer"/> class
        /// with the given options and <see cref="JobStorage.Current"/> storage.
        /// </summary>
        /// <param name="options">Server options</param>
        public BackgroundJobServer([NotNull] BackgroundJobServerOptions options)
            : this(options, JobStorage.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundJobServer"/> class
        /// with the specified options and the given storage.
        /// </summary>
        /// <param name="options">Server options</param>
        /// <param name="storage">The storage</param>
        public BackgroundJobServer([NotNull] BackgroundJobServerOptions options, [NotNull] JobStorage storage)
            : this(options, storage, Enumerable.Empty<IBackgroundProcess>())
        {
        }

        public BackgroundJobServer(
            [NotNull] BackgroundJobServerOptions options,
            [NotNull] JobStorage storage,
            [NotNull] IEnumerable<IBackgroundProcess> additionalProcesses)
        {
            if (storage == null) throw new ArgumentNullException("storage");
            if (options == null) throw new ArgumentNullException("options");
            if (additionalProcesses == null) throw new ArgumentNullException("additionalProcesses");

            _options = options;
            _lastEmailNotification = new DateTime();
            _hasFirstFailedDateTime = false;
            _failedJobsCount = 0;
            _failedLock = new object();

            var processes = new List<IBackgroundProcess>();
            processes.AddRange(GetRequiredProcesses());
            processes.AddRange(additionalProcesses);

            var properties = new Dictionary<string, object>
            {
                { "Queues", options.Queues },
                { "WorkerCount", options.WorkerCount }
            };

            Logger.Info("Starting Hangfire Server");
            Logger.InfoFormat("Using job storage: '{0}'.", storage);

            storage.WriteOptionsToLog(Logger);

            Logger.Info("Using the following options for Hangfire Server:");
            Logger.InfoFormat("    Worker count: {0}.", options.WorkerCount);
            Logger.InfoFormat("    Listening queues: {0}.", String.Join(", ", options.Queues.Select(x => "'" + x + "'")));
            Logger.InfoFormat("    Shutdown timeout: {0}.", options.ShutdownTimeout);
            Logger.InfoFormat("    Schedule polling interval: {0}.", options.SchedulePollingInterval);

            _processingServer = new BackgroundProcessingServer(
                storage,
                processes,
                properties,
                GetProcessingServerOptions());
        }

        public void Dispose()
        {
            _processingServer.Dispose();
            Logger.Info("Hangfire Server stopped.");
        }

        private IEnumerable<IBackgroundProcess> GetRequiredProcesses()
        {
            var processes = new List<IBackgroundProcess>();

            var filterProvider = _options.FilterProvider ?? JobFilterProviders.Providers;

            var factory = new BackgroundJobFactory(filterProvider);
            var performer = new BackgroundJobPerformer(filterProvider, _options.Activator ?? JobActivator.Current);
            var stateChanger = new BackgroundJobStateChanger(filterProvider);
            
            for (var i = 0; i < _options.WorkerCount; i++)
            {
                Worker worker = new Worker(_options.Queues, performer, stateChanger);
                worker.Subscribe(this);
                processes.Add(worker);
            }
            
            processes.Add(new DelayedJobScheduler(_options.SchedulePollingInterval, stateChanger));
            processes.Add(new RecurringJobScheduler(factory));

            return processes;
        }

        private BackgroundProcessingServerOptions GetProcessingServerOptions()
        {
            return new BackgroundProcessingServerOptions
            {
                ShutdownTimeout = _options.ShutdownTimeout,
                HeartbeatInterval = _options.HeartbeatInterval,
                ServerCheckInterval = _options.ServerWatchdogOptions != null
                    ? _options.ServerWatchdogOptions.CheckInterval
                    : _options.ServerCheckInterval,
                ServerTimeout = _options.ServerWatchdogOptions != null
                    ? _options.ServerWatchdogOptions.ServerTimeout
                    : _options.ServerTimeout
            };
        }

        [Obsolete("This method is a stub. There is no need to call the `Start` method. Will be removed in version 2.0.0.")]
        public void Start()
        {
        }

        [Obsolete("This method is a stub. Please call the `Dispose` method instead. Will be removed in version 2.0.0.")]
        public void Stop()
        {
        }

        public void OnNext(int i)
        {
            int threshold = int.Parse(ConfigurationManager.AppSettings["failedJobCountThreshold"]);
            int delay = int.Parse(ConfigurationManager.AppSettings["failedJobCheckInterval"]);
            TimeSpan duration = new TimeSpan(0, delay, 0);

            lock (_failedLock)
            {
                _failedJobsCount++;
            }            

            if (!_hasFirstFailedDateTime || DateTime.Now.Subtract(duration) > _lastEmailNotification)
            {
                _lastEmailNotification = DateTime.Now;
                _failedJobsCount = 1;
                _hasFirstFailedDateTime = true;
            }
            else if(_failedJobsCount > threshold )
            {
                string msg = "The server with machine name: \n";
                msg += '\n' + Environment.MachineName + ", ";
                msg += "\n \n has had a peak of failed jobs.";

                ServerStatusNotifier.Notify(1, msg);
                _hasFirstFailedDateTime = false;
            }
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }




    }
}
