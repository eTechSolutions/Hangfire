﻿using System;
using System.Globalization;
using System.Threading;
using HangFire.Client;
using HangFire.Common;
using HangFire.Server;
using HangFire.States;
using HangFire.Storage;
using Moq;
using Xunit;

namespace HangFire.Core.Tests
{
    public class PreserveCultureAttributeFacts
    {
        private readonly CreatedContext _createdContext;
        private readonly CreatingContext _creatingContext;
        private readonly PerformingContext _performingContext;
        private readonly PerformedContext _performedContext;
        private readonly Mock<IStorageConnection> _connection;
        private const string JobId = "id";

        public PreserveCultureAttributeFacts()
        {
            _connection = new Mock<IStorageConnection>();
            var job = Job.FromExpression(() => Sample());
            var state = new Mock<IState>();
            var stateMachineFactory = new Mock<IStateMachineFactory>();

            var createContext = new CreateContext(
                _connection.Object, stateMachineFactory.Object, job, state.Object);
            _creatingContext = new CreatingContext(createContext);
            _createdContext = new CreatedContext(createContext, false, null);

            var workerContext = new WorkerContext("server", new string[0], 1);

            var performContext = new PerformContext(workerContext, _connection.Object, JobId, job);
            _performingContext = new PerformingContext(performContext);
            _performedContext = new PerformedContext(performContext, false, null);
        }

        [Fact]
        public void OnCreating_ThrowsAnException_WhenContextIsNull()
        {
            var filter = CreateFilter();

            Assert.Throws<ArgumentNullException>(
                () => filter.OnCreating(null));
        }

        [Fact]
        public void OnCreating_CapturesCultures_AndSetsThemAsJobParameters()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("ru-RU");

            var filter = CreateFilter();
            filter.OnCreating(_creatingContext);

            Assert.Equal("ru-RU", _creatingContext.GetJobParameter<string>("CurrentCulture"));
            Assert.Equal("ru-RU", _creatingContext.GetJobParameter<string>("CurrentUICulture"));
        }

        [Fact]
        public void OnCreated_DoesNotThrowAnException()
        {
            var filter = CreateFilter();

            Assert.DoesNotThrow(() => filter.OnCreated(null));
        }

        [Fact]
        public void OnPerforming_SetsThreadCultures_ToTheSpecifiedOnesInJobParameters()
        {
            _connection.Setup(x => x.GetJobParameter(JobId, "CurrentCulture")).Returns("\"ru-RU\"");
            _connection.Setup(x => x.GetJobParameter(JobId, "CurrentUICulture")).Returns("\"ru-RU\"");

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            var filter = CreateFilter();
            filter.OnPerforming(_performingContext);

            Assert.Equal("ru-RU", Thread.CurrentThread.CurrentCulture.Name);
            Assert.Equal("ru-RU", Thread.CurrentThread.CurrentUICulture.Name);
        }

        [Fact]
        public void OnPerforming_DoesNotDoAnything_WhenCultureJobParameterIsNotSet()
        {
            _connection.Setup(x => x.GetJobParameter(JobId, "CurrentCulture")).Returns((string)null);
            _connection.Setup(x => x.GetJobParameter(JobId, "CurrentUICulture")).Returns((string)null);

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            var filter = CreateFilter();
            filter.OnPerforming(_performingContext);

            Assert.Equal("en-US", Thread.CurrentThread.CurrentCulture.Name);
            Assert.Equal("en-US", Thread.CurrentThread.CurrentUICulture.Name);
        }

        [Fact]
        public void OnPerformed_ThrowsAnException_WhenContextIsNull()
        {
            var filter = CreateFilter();

            Assert.Throws<ArgumentNullException>(() => filter.OnPerformed(null));
        }

        [Fact]
        public void OnPerformed_RestoresPreviousCurrentCulture()
        {
            _connection.Setup(x => x.GetJobParameter(JobId, "CurrentCulture")).Returns("\"ru-RU\"");
            _connection.Setup(x => x.GetJobParameter(JobId, "CurrentUICulture")).Returns("\"ru-RU\"");

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            var filter = CreateFilter();
            filter.OnPerforming(_performingContext);
            filter.OnPerformed(_performedContext);

            Assert.Equal("en-US", Thread.CurrentThread.CurrentCulture.Name);
            Assert.Equal("en-US", Thread.CurrentThread.CurrentUICulture.Name);
        }

        [Fact]
        public void OnPerformed_RestoresPreviousCurrentCulture_OnlyIfItWasChanged()
        {
            _connection.Setup(x => x.GetJobParameter(JobId, "CurrentCulture")).Returns((string)null);
            _connection.Setup(x => x.GetJobParameter(JobId, "CurrentUICulture")).Returns((string)null);

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            var filter = CreateFilter();
            filter.OnPerforming(_performingContext);
            filter.OnPerformed(_performedContext);

            Assert.Equal("en-US", Thread.CurrentThread.CurrentCulture.Name);
            Assert.Equal("en-US", Thread.CurrentThread.CurrentUICulture.Name);
        }

        public static void Sample() { }

        private PreserveCultureAttribute CreateFilter()
        {
            return new PreserveCultureAttribute();
        }
    }
}
