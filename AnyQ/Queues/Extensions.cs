using AnyQ.Jobs;

namespace AnyQ.Queues {
    internal static class Extensions {

        public static QueueCreationOptions ToQueueCreationOptions(this HandlerConfiguration handlerConfiguration) {
            return new QueueCreationOptions {
                QueueId = handlerConfiguration.QueueId,
                QueueName = handlerConfiguration.QueueName,
                ThrottleInterval = handlerConfiguration.ThrottleInterval
            };
        }
    }
}
