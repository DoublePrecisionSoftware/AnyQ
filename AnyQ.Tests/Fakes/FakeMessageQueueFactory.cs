using AnyQ.Queues;
using Moq;
using Ploeh.AutoFixture;
using System.Collections.Generic;

namespace AnyQ.Tests.Fakes {
    public class FakeMessageQueueFactory : IMessageQueueFactory {
        public readonly Dictionary<string, Mock<IMessageQueue>> MockQueues = new Dictionary<string, Mock<IMessageQueue>>();
        private readonly IFixture _fixture;

        public FakeMessageQueueFactory(IFixture fixture) {
            _fixture = fixture;
        }

        public Mock<IMessageQueue> CreateMock(QueueCreationOptions options) {
            var mock = _fixture.Create<Mock<IMessageQueue>>();
            mock.SetupGet(m => m.Id).Returns(options.QueueId);
            MockQueues.Add(options.QueueId, mock);
            return mock;
        }

        public IMessageQueue Create(QueueCreationOptions options) {
            return CreateMock(options).Object;
        }

        public bool Exists(string queueId) => true;
    }
}
