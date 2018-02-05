using AnyQ.Formatters;
using AnyQ.Jobs;
using AnyQ.Queues;
using Moq;
using Ploeh.AutoFixture;
using System.Collections.Generic;

namespace AnyQ.Tests.Fakes {
    public partial class FakeJobQueueFactory : IJobQueueFactory {
        private readonly IFixture _fixture;
        
        public readonly FakeMessageQueueFactory MockMessageQueueFactory;
        public readonly Mock<IMessageFactory> MockMessageFactory;
        public readonly Mock<IPayloadFormatter> MockFormatter;
        public readonly Mock<IRequestSerializer> MockSerializer;

        public readonly List<JobQueue> _jobQueues;

        public FakeJobQueueFactory(IFixture fixture) {
            _fixture = fixture;

            _jobQueues = new List<JobQueue>();

            MockMessageQueueFactory = new FakeMessageQueueFactory(_fixture);

            MockMessageFactory = _fixture.Freeze<Mock<IMessageFactory>>();
            MockFormatter = _fixture.Freeze<Mock<IPayloadFormatter>>();
            MockSerializer = _fixture.Freeze<Mock<IRequestSerializer>>();
        }

        public JobQueue Create(string queueId) {
            var jobQueue = new JobQueue(
                MockMessageQueueFactory,
                MockMessageFactory.Object,
                MockFormatter.Object,
                MockSerializer.Object,
                _fixture.Build<HandlerConfiguration>()
                    .With(c => c.QueueId, queueId)
                    .Create());
            _jobQueues.Add(jobQueue);
            return jobQueue;
        }
        public JobQueue Create(HandlerConfiguration handlerConfiguration) {
            var jobQueue = new JobQueue(
                MockMessageQueueFactory,
                MockMessageFactory.Object,
                MockFormatter.Object,
                MockSerializer.Object,
                handlerConfiguration);
            _jobQueues.Add(jobQueue);
            return jobQueue;
        }
    }
}
