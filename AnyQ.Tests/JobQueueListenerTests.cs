using AnyQ.Jobs;
using AnyQ.Queues;
using AnyQ.Queues.Status;
using AnyQ.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace AnyQ.Tests {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    [TestClass]
    public partial class JobQueueListenerTests {
        
        private IFixture _fixture;
        private FakeJobQueueFactory _jobQueueFactory;
        private Mock<ListenerConfiguration> _mockListenerConfiguration;
        private Mock<IStatusProvider> _mockStatusProvider;
        private JobQueueListener _sut;

        [TestInitialize]
        public void SetupMocks() {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _jobQueueFactory = new FakeJobQueueFactory(_fixture);
            _mockListenerConfiguration = _fixture.Freeze<Mock<ListenerConfiguration>>();
            _mockStatusProvider = _fixture.Freeze<Mock<IStatusProvider>>();
            _sut = new JobQueueListener(_jobQueueFactory, _mockListenerConfiguration.Object);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Contructor_Gaurds_Against_Null_JobQueueFactory() {
            var sut = new JobQueueListener(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddHandler_Gaurds_Against_Nulls() {
            _sut.AddHandler(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddHandlerLocator_Gaurds_Against_Nulls() {
            _sut.AddHandlerLocator(null);
        }

        [TestMethod]
        public void Listen_Starts_BeginRecieve_On_Queue() {

            var mockHandler = _sut.AddMockHandler(_fixture);

            _sut.Listen();

            _jobQueueFactory.MockMessageQueueFactory.MockQueues[mockHandler.Object.Configuration.QueueId]
                .Verify(m => m.BeginReceive(), Times.Once());
        }

        [TestMethod]
        public void Listen_Returns_If_Already_Listening() {

            var mockHandler = _sut.AddMockHandler(_fixture);

            _sut.Listen();
            _sut.Listen();

            _jobQueueFactory.MockMessageQueueFactory.MockQueues[mockHandler.Object.Configuration.QueueId]
                .Verify(m => m.BeginReceive(), Times.Once());
        }

        [TestMethod]
        public void AddStatusProvider_Adds_Provider() {

            var mockHandler = _sut.AddMockHandler(_fixture);

            var mockStatusProvider = _fixture.Create<Mock<IStatusProvider>>();
            _sut.AddStatusProvider(mockStatusProvider.Object);

            _sut.ExecuteJobAsync(mockHandler.Object.Configuration.QueueId, "test");

            _mockStatusProvider.Verify(m => m.WriteStatus(It.IsAny<JobStatus>()), Times.AtLeastOnce());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddStatusProvider_Guards_Null() {
            _sut.AddStatusProvider(null);
        }

        [TestMethod]
        public void AddHandler_Creates_New_Queue() {

            var mockHandler = _sut.AddMockHandler(_fixture);

            Assert.IsTrue(_jobQueueFactory.MockMessageQueueFactory.MockQueues.Count == 1);
        }

        [TestMethod]
        public void AddHandler_Creates_Only_One_Queue_Per_QueueId() {
            var mockHandler = _fixture.Create<Mock<JobHandler>>();

            mockHandler.Setup(m => m.CanProcess(It.IsAny<ProcessingRequest>())).Returns(true);
            mockHandler.SetupGet(p => p.Configuration).Returns(_fixture.Create<HandlerConfiguration>());
            
            _sut.AddHandler(mockHandler.Object);
            _sut.AddHandler(mockHandler.Object);

            Assert.IsTrue(_jobQueueFactory.MockMessageQueueFactory.MockQueues.Count == 1);
        }

        [TestMethod]
        public void AddHandlerLocator_Creates_New_Queue() {
            var handlerLocator = new FakeJobHandlerLocator(_fixture);

            _sut.AddHandlerLocator(handlerLocator);

            Assert.IsTrue(_jobQueueFactory.MockMessageQueueFactory.MockQueues.Count == 1);
        }

        [TestMethod]
        public void GetMessage_Calls_Queue_GetMessage() {

            var mockHandler = _sut.AddMockHandler(_fixture);

            var result = _sut.GetMessage(mockHandler.Object.Configuration.QueueId, "test");

            Assert.IsNotNull(result);
            _jobQueueFactory.MockMessageQueueFactory.MockQueues[mockHandler.Object.Configuration.QueueId]
                .Verify(m => m.GetMessage("test"), Times.Once());
        }

        [TestMethod]
        public void GetMessages_Calls_Queue_GetMessages() {

            var mockHandler = _sut.AddMockHandler(_fixture);

            var result = _sut.GetMessages(mockHandler.Object.Configuration.QueueId);

            Assert.IsNotNull(result);
            _jobQueueFactory.MockMessageQueueFactory.MockQueues[mockHandler.Object.Configuration.QueueId]
                .Verify(m => m.GetMessages(), Times.Once());
        }

        [TestMethod]
        public void ReceiveMessage_Calls_Queue_Empty_ReceiveMessage() {

            var mockHandler = _sut.AddMockHandler(_fixture);

            _sut.ReceiveMessage(mockHandler.Object.Configuration.QueueId);

            _jobQueueFactory.MockMessageQueueFactory.MockQueues[mockHandler.Object.Configuration.QueueId]
                .Verify(m => m.Receive(), Times.Once());
        }

        [TestMethod]
        public void ReceiveMessage_Calls_Queue_ReceiveMessage() {

            var mockHandler = _sut.AddMockHandler(_fixture);

            _sut.ReceiveMessage(mockHandler.Object.Configuration.QueueId, "test");

            _jobQueueFactory.MockMessageQueueFactory.MockQueues[mockHandler.Object.Configuration.QueueId]
                .Verify(m => m.Receive("test"), Times.Once());
        }

        [TestMethod]
        public void PurgeQueue_Calls_Queue_Purge() {

            var mockHandler = _sut.AddMockHandler(_fixture);

            _sut.PurgeQueue(mockHandler.Object.Configuration.QueueId);

            _jobQueueFactory.MockMessageQueueFactory.MockQueues[mockHandler.Object.Configuration.QueueId]
                .Verify(m => m.Purge(), Times.Once());
        }

        [TestMethod]
        public void DeleteMessage_Calls_Queue_DeleteMessage() {

            var mockHandler = _sut.AddMockHandler(_fixture);

            _sut.DeleteMessage(mockHandler.Object.Configuration.QueueId, "test");

            _jobQueueFactory.MockMessageQueueFactory.MockQueues[mockHandler.Object.Configuration.QueueId]
                .Verify(m => m.DeleteMessage("test"), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(QueueNotAvailableException))]
        public void InternalJobQueueFactory_Throws_QueueNotAvailableException_For_Missing_Queue() {
            _sut.ExecuteJobAsync("x", "test");
        }

        [TestMethod]
        public void ExecuteJobAsync_Calls_Queue_GetMessage() {

            var mockHandler = _sut.AddMockHandler(_fixture);

            _sut.ExecuteJobAsync(mockHandler.Object.Configuration.QueueId, "test");

            _jobQueueFactory.MockMessageQueueFactory.MockQueues[mockHandler.Object.Configuration.QueueId]
                .Verify(m => m.GetMessage("test"), Times.Once());
        }

        [TestMethod]
        public void ExecuteJobAsync_Reports_Status() {

            var mockHandler = _sut.AddMockHandler(_fixture);

            _sut.AddStatusProvider(_mockStatusProvider.Object);

            _sut.ExecuteJobAsync(mockHandler.Object.Configuration.QueueId, "test");

            _mockStatusProvider.Verify(m => m.WriteStatus(It.IsAny<JobStatus>()), Times.AtLeastOnce());
        }

        [TestMethod]
        public void ExecuteJobAsync_Calls_Handler_ProcessAsync() {

            var mockHandler = _sut.AddMockHandler(_fixture);

            _sut.ExecuteJobAsync(mockHandler.Object.Configuration.QueueId, "test");

            mockHandler.Verify(m => m.ProcessAsync(It.IsAny<ProcessingRequest>(), It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [TestMethod]
        public void ExecuteJobAsync_Does_Not_Call_Handler_ProcessAsync_After_Stop() {

            var mockHandler = _sut.AddMockHandler(_fixture);

            _sut.AddStatusProvider(_mockStatusProvider.Object);
            _sut.Stop(true);
            _sut.ExecuteJobAsync(mockHandler.Object.Configuration.QueueId, "test");

            mockHandler.Verify(m => m.ProcessAsync(It.IsAny<ProcessingRequest>(), It.IsAny<CancellationToken>()),
                Times.Never());
            _mockStatusProvider.Verify(m => m.WriteStatus(It.IsAny<JobStatus>()), Times.Exactly(2));
        }

        [TestMethod]
        [ExpectedException(typeof(QueueNotAvailableException))]
        public void ExecuteJobAsync_Throws_For_Missing_Queue() {

            var mockHandler = _sut.AddMockHandler(_fixture);

            _sut.AddStatusProvider(_mockStatusProvider.Object);

            _sut.ExecuteJobAsync("test", "test");
        }

        [TestMethod]
        public void HandleRequestAsync_Skips_Unprocessable_Jobs() {

            var mockHandler = _sut.AddMockHandler(_fixture);
            mockHandler.Setup(m => m.CanProcess(It.IsAny<ProcessingRequest>())).Returns(false);

            _sut.AddStatusProvider(_mockStatusProvider.Object);

            _sut.ExecuteJobAsync(mockHandler.Object.Configuration.QueueId, "test");

            _mockStatusProvider.Verify(m => m.WriteStatus(It.Is<JobStatus>(s => s.Status == JobStatus.Skipped)), Times.Once());
        }

        [TestMethod]
        public void Stop_Calls_EndRecieve_On_Queues() {

            var mockHandler = _sut.AddMockHandler(_fixture);

            _sut.Stop();

            _jobQueueFactory.MockMessageQueueFactory.MockQueues[mockHandler.Object.Configuration.QueueId]
                .Verify(m => m.EndRecieve(), Times.Once());
        }

        [TestMethod]
        public void SendJob_Calls_Send_On_Queue() {

            var mockHandler = _sut.AddMockHandler(_fixture);

            var msg = _sut.SendJob(mockHandler.Object.Configuration.QueueId, "test", new object(), "test");

            _jobQueueFactory.MockMessageQueueFactory.MockQueues[mockHandler.Object.Configuration.QueueId]
                .Verify(m => m.Send(It.IsAny<IMessage>()), Times.Once());
        }

        [TestMethod]
        public void ExecuteJob_Calls_Receive_On_Queue() {

            var mockHandler = _sut.AddMockHandler(_fixture);

            _sut.ExecuteJob(mockHandler.Object.Configuration.QueueId, "test");

            _jobQueueFactory.MockMessageQueueFactory.MockQueues[mockHandler.Object.Configuration.QueueId]
                .Verify(m => m.Receive("test"), Times.Once());
        }

        [TestMethod]
        public void ExecuteJob_Calls_Receive_On_Queue_NoMessageId() {

            var mockHandler = _sut.AddMockHandler(_fixture);

            _sut.ExecuteJob(mockHandler.Object.Configuration.QueueId);

            _jobQueueFactory.MockMessageQueueFactory.MockQueues[mockHandler.Object.Configuration.QueueId]
                .Verify(m => m.Receive(), Times.Once());
        }

        [TestMethod]
        [Timeout(2000)]
        public void ProcessingCompleted_Raised_On_Successful_Processing() {

            var mockHandler = _sut.AddFakeHandler(_fixture.Create<HandlerConfiguration>(), true);

            var tcs = new TaskCompletionSource<ProcessingCompletedEventArgs>();

            _sut.ProcessingCompleted += (s, e) => {
                tcs.SetResult(e);
            };

            _jobQueueFactory.MockMessageQueueFactory.MockQueues[mockHandler.Configuration.QueueId]
                .Raise(m => m.ReceivedMessage += null, new ReceivedMessageEventArgs(_fixture.Create<IMessage>()));

            tcs.Task.Wait();

            Assert.IsNotNull(tcs.Task.Result);
        }

        [TestMethod]
        public void HandleRequestAsync_Skips_Missing_Handlers() {

            var locator = new FakeJobHandlerLocator(_fixture);
            var fakeHandler = locator.AddFakeHandler(_fixture.Create<HandlerConfiguration>());

            _sut.AddStatusProvider(_mockStatusProvider.Object);

            _sut.AddHandlerLocator(locator);
            
            locator.RemoveHandler(fakeHandler.Configuration.QueueId);

            _sut.ExecuteJobAsync(fakeHandler.Configuration.QueueId, "test").Wait();

            _mockStatusProvider.Verify(m => m.WriteStatus(It.Is<JobStatus>(s => s.Status == JobStatus.Skipped)), Times.Once());
        }

        [TestMethod]
        [Timeout(2000)]
        public void ProcessingFailed_Raised_On_Failed_Processing() {

            var mockHandler = _sut.AddFakeHandler(_fixture.Create<HandlerConfiguration>(), true, false, true);

            var tcs = new TaskCompletionSource<ProcessingFailedEventArgs>();

            _sut.ProcessingFailed += (s, e) => {
                tcs.SetResult(e);
            };

            _jobQueueFactory.MockMessageQueueFactory.MockQueues[mockHandler.Configuration.QueueId]
                .Raise(m => m.ReceivedMessage += null, new ReceivedMessageEventArgs(_fixture.Create<IMessage>()));

            tcs.Task.Wait();

            Assert.IsNotNull(tcs.Task.Result);
        }

        [TestMethod]
        [Timeout(5000)]
        public void ProcessingTimedOut_Raised_On_Hung_Job() {

            var config = new ListenerConfiguration {
                JobTimeout = 2000
            };

            var sut = new JobQueueListener(_jobQueueFactory, config);

            var mockHandler = sut.AddFakeHandler(_fixture.Create<HandlerConfiguration>(), true, true);

            var tcs = new TaskCompletionSource<ProcessingFailedEventArgs>();

            sut.ProcessingTimedOut += (s, e) => {
                tcs.SetResult(e);
            };

            _jobQueueFactory.MockMessageQueueFactory.MockQueues[mockHandler.Configuration.QueueId]
                .Raise(m => m.ReceivedMessage += null, new ReceivedMessageEventArgs(_fixture.Create<IMessage>()));

            tcs.Task.Wait();

            Assert.IsNotNull(tcs.Task.Result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddRedirectStrategy_Guards_Null_QueueId() {
            _sut.AddRedirectStrategy(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddRedirectStrategy_Guards_Null_RedirectStrategy() {
            _sut.AddRedirectStrategy("test", null);
        }

        [TestMethod]
        [Timeout(2000)]
        public void Redirect_Executed_On_Specified_Status() {

            var fakeHandler = new FakeJobHandler(
                _fixture.Create<HandlerConfiguration>(), true, false, true);
            var failedHandler = new FakeJobHandler(
                _fixture.Build<HandlerConfiguration>().With(m => m.QueueId, "failed").Create(), true, false, false);
            var newQueue = "failed";

            fakeHandler.AddRedirect(RedirectStrategy.Create((s, e) => s.Status == JobStatus.Failed, s => newQueue));

            _sut.AddHandler(fakeHandler);
            _sut.AddHandler(failedHandler);

            var tcs = new TaskCompletionSource<RequestRedirectedEventArgs>();

            _sut.RequestRedirected += (s, e) => {
                tcs.SetResult(e);
            };

            _jobQueueFactory.MockMessageQueueFactory.MockQueues[fakeHandler.Configuration.QueueId]
                .Raise(m => m.ReceivedMessage += null, new ReceivedMessageEventArgs(_fixture.Create<IMessage>()));

            tcs.Task.Wait();

            Assert.AreEqual(newQueue, tcs.Task.Result.ToQueue);
        }


        [TestMethod]
        [Timeout(2000)]
        public void AddRedirectStrategy_Overwrites_Previous_For_Queue() {

            var locator = new FakeJobHandlerLocator(_fixture);
            var fakeHandler = locator.AddFakeHandler(_fixture.Create<HandlerConfiguration>(), true, false, true);
            var failedHandler = locator.AddFakeHandler(_fixture.Build<HandlerConfiguration>().With(m => m.QueueId, "failed").Create(), true, false, false);
            var newQueue = "failed";

            fakeHandler.AddRedirect(RedirectStrategy.Create((s, e) => s.Status == JobStatus.Failed, s => "x"));

            _sut.AddHandlerLocator(locator);
            _sut.AddRedirectStrategy(fakeHandler.Configuration.QueueId, RedirectStrategy.Create((s, e) => s.Status == JobStatus.Failed, s => newQueue));

            var tcs = new TaskCompletionSource<RequestRedirectedEventArgs>();

            _sut.RequestRedirected += (s, e) => {
                tcs.SetResult(e);
            };

            _jobQueueFactory.MockMessageQueueFactory.MockQueues[fakeHandler.Configuration.QueueId]
                .Raise(m => m.ReceivedMessage += null, new ReceivedMessageEventArgs(_fixture.Create<IMessage>()));

            tcs.Task.Wait();

            Assert.AreEqual(newQueue, tcs.Task.Result.ToQueue);
        }

        //[TestMethod]
        //[ExpectedException(typeof(ObjectDisposedException))]
        //public void Dispose_Disposes_Of_Handlers_And_Queues() {

        //    var mockHandler = _sut.AddMockHandler(_fixture);
        //    _sut.Dispose();
        //    _jobQueueFactory.MockQueue.Object.Dispose();
        //}
        
        // TODO: test StatusProvider exceptions get swallowed
    }
}