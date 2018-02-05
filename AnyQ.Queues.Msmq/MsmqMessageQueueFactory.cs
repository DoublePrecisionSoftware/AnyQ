using System.Messaging;

namespace AnyQ.Queues.Msmq {
    /// <summary>
    /// Represents a factory for creating MSMQ Message queues
    /// </summary>
    public class MsmqMessageQueueFactory : IMessageQueueFactory {
        private readonly AccessControlList _accessControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsmqMessageQueueFactory"/> class
        /// </summary>
        /// <param name="accessControl">List of access controls for created queues</param>
        public MsmqMessageQueueFactory(AccessControlList accessControl) {
            _accessControl = accessControl;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MsmqMessageQueue"/> class for the specified queueId
        /// </summary>
        /// <param name="options">Options for creating the queue</param>
        /// <exception cref="ServiceNotAvailableException" />
        /// <exception cref="MessageQueueException" />
        public IMessageQueue Create(QueueCreationOptions options) {
            try {
                return new MsmqMessageQueue(options, false, _accessControl);
            } catch (MessageQueueException ex) {
                if (ex.MessageQueueErrorCode == MessageQueueErrorCode.ServiceNotAvailable) {
                    throw new ServiceNotAvailableException("Cannot create queue", ex);
                }
                throw;
            }
        }

        /// <summary>
        /// Returns true if a queue with the specified <paramref name="queueId"/> exists
        /// </summary>
        /// <param name="queueId">Unique identifier for the queue</param>
        /// <exception cref="MessageQueueException" />
        /// <exception cref="System.InvalidOperationException" />
        public bool Exists(string queueId) {
            return MessageQueue.Exists(queueId);
        }
    }
}
