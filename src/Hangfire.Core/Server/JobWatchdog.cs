using System;

namespace Hangfire.Server
{
    class JobWatchdog : IBackgroundProcess
    {
        public static readonly TimeSpan DefaultCheckInterval = TimeSpan.FromMinutes(5);

        private readonly TimeSpan _checkInterval;

        public JobWatchdog(TimeSpan checkInterval)
        {

            _checkInterval = checkInterval;
        }

        public void Execute(BackgroundProcessContext context)
        {
            // SQL query for the number of failed jobs since last check

            using (var connection = context.Storage.GetConnection())
            {
                connection.Heartbeat(context.ServerId);
            }

            context.Wait(_checkInterval); ;
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}