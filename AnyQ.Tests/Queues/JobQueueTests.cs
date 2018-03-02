using AnyQ.Formatters;
using AnyQ.Jobs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using System.IO;
using System.Threading.Tasks;

namespace AnyQ.Queues.Tests {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    [TestClass]
    public class JobQueueTests {

        private Mock<IMessageQueueFactory> _mockMessageQueueFactory;
        private Mock<IMessageFactory> _mockMessageFactory;
        private Mock<IPayloadFormatter> _mockPayloadFormatter;
        private Mock<IRequestSerializer> _mockRequestSerializer;
        private Mock<HandlerConfiguration> _mockHandlerConfiguration;
        private Mock<IMessageQueue> _mockQueue;
        private IFixture _fixture;
        private JobQueue _sut;

        [TestInitialize]
        public void SetupSut() {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockMessageQueueFactory = _fixture.Freeze<Mock<IMessageQueueFactory>>();
            _mockMessageFactory = _fixture.Freeze<Mock<IMessageFactory>>();
            _mockPayloadFormatter = _fixture.Freeze<Mock<IPayloadFormatter>>();
            _mockRequestSerializer = _fixture.Freeze<Mock<IRequestSerializer>>();
            _mockHandlerConfiguration = _fixture.Freeze<Mock<HandlerConfiguration>>();
            _mockQueue = _fixture.Freeze<Mock<IMessageQueue>>();

            _mockMessageQueueFactory.Setup(m => m.Create(It.IsAny<QueueCreationOptions>()))
                .ReturnsUsingFixture(_fixture);

            _sut = new JobQueue(
                _mockMessageQueueFactory.Object,
                _mockMessageFactory.Object,
                _mockPayloadFormatter.Object,
                _mockRequestSerializer.Object,
                _mockHandlerConfiguration.Object
            );
        }

        [TestMethod]
        public void StartListening_Should_Call_BeginReceive_On_Queue() {

            _sut.StartListening();

            _mockQueue.Verify(m => m.BeginReceive(), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void Constructor_Guards_Null_HandlerConfiguration() {
            var queue = new JobQueue(null, null, null, null, null);
        }

        [TestMethod]
        public void Close_Should_Call_EndRecieve_On_Queue() {

            _sut.Close();

            _mockQueue.Verify(m => m.EndRecieve(), Times.Once());
        }

        [TestMethod]
        public void SendJob_Should_Call_Send_On_Queue() {

            _sut.SendJob("test", new object(), "test");

            _mockQueue.Verify(m => m.Send(It.IsAny<IMessage>()), Times.Once());
        }

        [TestMethod]
        public void GetProcessingRequest_Should_Return_ProcessingRequest() {

            _mockQueue.Setup(m => m.GetMessage(It.IsAny<string>()))
                .ReturnsUsingFixture(_fixture);
            _mockRequestSerializer.Setup(m => m.Deserialize(It.IsAny<byte[]>()))
                .ReturnsUsingFixture(_fixture);

            var request = _sut.GetProcessingRequest("test");

            Assert.IsNotNull(request);
        }

        [TestMethod]
        public void ReceiveMessage_Should_Call_Receive_On_Queue() {

            var testId = "test";

            _mockQueue.Setup(m => m.Receive(testId));

            _sut.ReceiveMessage(testId);

            _mockQueue.Verify(m => m.Receive(testId), Times.Once());
        }

        [TestMethod]
        public void Empty_ReceiveMessage_Should_Call_Empty_Receive_On_Queue() {

            _mockQueue.Setup(m => m.Receive());

            _sut.ReceiveMessage();

            _mockQueue.Verify(m => m.Receive(), Times.Once());
        }

        [TestMethod]
        public void Purge_Should_Call_Purge_On_Queue() {

            _mockQueue.Setup(m => m.Purge());

            _sut.Purge();

            _mockQueue.Verify(m => m.Purge(), Times.Once());
        }

        [TestMethod]
        public void GetMessages_Should_Call_GetMessages_On_Queue() {

            _mockQueue.Setup(m => m.GetMessages())
                .ReturnsUsingFixture(_fixture);

            var result = _sut.GetMessages();

            Assert.IsNotNull(result);
            _mockQueue.Verify(m => m.GetMessages(), Times.Once());
        }

        [TestMethod]
        public void GetMessage_Should_Call_GetMessage_On_Queue() {

            var testId = "test";

            _mockQueue.Setup(m => m.GetMessage(testId));

            _sut.GetMessage(testId);

            _mockQueue.Verify(m => m.GetMessage(testId), Times.Once());
        }

        [TestMethod]
        public void DeleteMessage_Should_Call_DeleteMessage_On_Queue() {

            var testId = "test";

            _mockQueue.Setup(m => m.DeleteMessage(testId));

            _sut.DeleteMessage(testId);

            _mockQueue.Verify(m => m.DeleteMessage(testId), Times.Once());
        }

        [TestMethod]
        public void ReceivedJob_Should_Call_OnReceivedJob() {

            var tcs = new TaskCompletionSource<JobReceivedEventArgs>();

            _sut.JobReceived += (s, e) => {
                tcs.SetResult(e);
            };

            _mockQueue.Raise(m => m.ReceivedMessage += null, new ReceivedMessageEventArgs(_fixture.Create<IMessage>()));

            tcs.Task.Wait();

            Assert.IsNotNull(tcs.Task.Result);
        }

        [TestMethod]
        public void Dispose_Should_Call_Dispose_On_Queue() {

            _sut.Dispose();

            _mockQueue.Verify(m => m.Dispose(), Times.Once());
        }
    }
}