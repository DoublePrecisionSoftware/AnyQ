using AnyQ.Formatters;
using AnyQ.Jobs;
using System;

namespace AnyQ.Queues.Msmq {
    public class MsmqJobQueueFactory : IJobQueueFactory {
        private MsmqMessageQueueFactory _queueFactory;
        private MsmqMessageFactory _messageFactory;
        private IPayloadFormatter _payloadFormatter;
        private IRequestSerializer _requestSerializer;

        public MsmqJobQueueFactory(
            IPayloadFormatter payloadFormatter, 
            IRequestSerializer requestSerializer,
            System.Messaging.AccessControlList accessControlList = null) {

            _queueFactory = new MsmqMessageQueueFactory(accessControlList);
            _messageFactory = new MsmqMessageFactory();
            _payloadFormatter = payloadFormatter;
            _requestSerializer = requestSerializer;
        }

        public JobQueue Create(string queueId) {
            throw new NotImplementedException("Deprecated");
        }

        public JobQueue Create(HandlerConfiguration handlerConfiguration) {
            return new JobQueue(_queueFactory, _messageFactory, _payloadFormatter, _requestSerializer, handlerConfiguration);
        }
    }
}
