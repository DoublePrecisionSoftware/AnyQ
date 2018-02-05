using AnyQ.Jobs;
using Moq;
using Ploeh.AutoFixture;

namespace AnyQ.Tests {
    internal static class TestExtensions {
        public static Mock<JobHandler> AddMockHandler(this JobQueueListener listener, IFixture fixture) {
            var mockHandler = fixture.Freeze<Mock<JobHandler>>();

            mockHandler.Setup(m => m.CanProcess(It.IsAny<ProcessingRequest>())).Returns(true);
            mockHandler.SetupGet(p => p.Configuration).Returns(fixture.Create<HandlerConfiguration>());

            listener.AddHandler(mockHandler.Object);

            return mockHandler;
        }

        public static Fakes.FakeJobHandler AddFakeHandler(
            this JobQueueListener listener,
            HandlerConfiguration handlerConfiguration, 
            bool canProcess, 
            bool hangOnProcessing = false,
            bool throwOnProcessing = false) {
            var fakeHandler = new Fakes.FakeJobHandler(handlerConfiguration, canProcess, hangOnProcessing, throwOnProcessing);

            listener.AddHandler(fakeHandler);

            return fakeHandler;
        }
    }
}
