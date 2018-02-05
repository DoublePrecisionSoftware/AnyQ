using AnyQ.Jobs;
using Moq;
using Ploeh.AutoFixture;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AnyQ.Tests.Fakes {

    public class FakeJobHandlerLocator : IJobHandlerLocator {
        private readonly IFixture _fixture;
        private readonly Dictionary<string, JobHandler> _handlers = new Dictionary<string, JobHandler>();

        public FakeJobHandlerLocator(IFixture fixture) {
            _fixture = fixture;
            AddMockHandler();
        }

        internal void RemoveHandler(string queueId) {
            _handlers.Remove(queueId);
        }

        internal Mock<JobHandler> AddMockHandler() {
            var mockHandler = _fixture.Create<Mock<JobHandler>>();

            mockHandler.Setup(m => m.CanProcess(It.IsAny<ProcessingRequest>()))
                .Returns(true);
            mockHandler.SetupGet(m => m.Configuration)
                .Returns(_fixture.Create<HandlerConfiguration>());
            mockHandler.Setup(m => m.ProcessAsync(It.IsAny<ProcessingRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            _handlers.Add(mockHandler.Object.Configuration.QueueId, mockHandler.Object);

            return mockHandler;
        }

        internal FakeJobHandler AddFakeHandler(HandlerConfiguration handlerConfiguration, bool canProcess = true, bool hangOnProcessing = false, bool throwOnProcessing = false) {

            var fakeHandler = new FakeJobHandler(handlerConfiguration, canProcess, hangOnProcessing, throwOnProcessing);

            _handlers.Add(fakeHandler.Configuration.QueueId, fakeHandler);

            return fakeHandler;
        }

        public IEnumerable<JobHandler> GetHandlers() => _handlers.Values;

        public bool TryGetHandlerByQueueId(string queueId, out JobHandler handler) {
            if (_handlers.TryGetValue(queueId, out handler)) {
                return true;
            }
            return false;
        }
    }
}
