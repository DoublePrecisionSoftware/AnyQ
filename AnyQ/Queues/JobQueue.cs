using AnyQ.Formatters;
using AnyQ.Jobs;
using System;
using System.Collections.Generic;

namespace AnyQ.Queues {

    /// <summary>
    /// Represents a wrapper around an <see cref="IMessageQueue"/> for handling serialization of payloads
    /// and creation of <see cref="ProcessingRequest"/>s
    /// </summary>
    public class JobQueue : IDisposable {
        //private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IMessageQueue _queue;
        private readonly IMessageQueueFactory _messageQueueFactory;
        private readonly IMessageFactory _messageFactory;
        private readonly IPayloadFormatter _formatter;
        private readonly IRequestSerializer _serializer;
        private bool _disposedValue = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobQueue"/> class
        /// </summary>
        /// <param name="queueFactory">Factory for creating the queue</param>
        /// <param name="messageFactory">Factory for creating new messages</param>
        /// <param name="formatter">Formatter for formatting payloads</param>
        /// <param name="serializer">Serializer for serializing requests</param>
        /// <param name="handlerConfiguration">Configuration creating handlers (used to copy into a <see cref="QueueCreationOptions"/> object</param>
        public JobQueue(
            IMessageQueueFactory queueFactory, 
            IMessageFactory messageFactory, 
            IPayloadFormatter formatter,
            IRequestSerializer serializer,
            HandlerConfiguration handlerConfiguration) {
            if (handlerConfiguration == null) {
                throw new ArgumentNullException(nameof(handlerConfiguration));
            }

            _messageQueueFactory = queueFactory ?? throw new ArgumentNullException(nameof(queueFactory));
            _messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            _queue = _messageQueueFactory.Create(handlerConfiguration.ToQueueCreationOptions());
            _queue.ReceivedMessage += OnReceivedJob;
        }

        /// <summary>
        /// Unique identifier for the queue
        /// </summary>
        public string QueueId => _queue.Id;

        /// <summary>
        /// Begin listening for <see cref="IMessage"/>s on the <see cref="IMessageQueue"/>
        /// </summary>
        public void StartListening() {
            _queue.BeginReceive();
        }

        /// <summary>
        /// Fired when a job is received
        /// </summary>
        public virtual event EventHandler<JobReceivedEventArgs> JobReceived;

        /// <summary>
        /// Executed when a job arrives on the queue
        /// </summary>
        /// <param name="sender">Sender of event</param>
        /// <param name="e">Event data</param>
        protected virtual void OnReceivedJob(object sender, ReceivedMessageEventArgs e) {
            var message = e.Message;
            var jobId = e.Message.Id;

            var jobRequest = CreateJobRequest(e.Message);
            var processingRequest = CreateProcessingRequest(jobRequest, e.Message);

            JobReceived?.Invoke(this, new JobReceivedEventArgs(processingRequest));
        }

        private ProcessingRequest CreateProcessingRequest(JobRequest request, IMessage message) {
            return new ProcessingRequest {
                JobId = message.Id,
                Name = message.Label,
                JobRequest = request,
                QueueId = _queue.Id,
                QueueName = _queue.Name,
            };
        }

        private JobRequest CreateJobRequest(IMessage message) {
            return _serializer.Deserialize(message.Body);
        }

        /// <summary>
        /// Send a new <see cref="JobRequest"/> (serialized into an <see cref="IMessage"/>) to the queue handled by this instance
        /// </summary>
        /// <param name="type">Type of request</param>
        /// <param name="payload">Data to pass into generated <see cref="IMessage"/></param>
        /// <param name="messageLabel">Human-readable name for generated <see cref="IMessage"/></param>
        public virtual IMessage SendJob(string type, object payload, string messageLabel) {
            var request = new JobRequest {
                Type = type,
                Payload = _formatter.Write(payload),
            };

            var bodyStream = _serializer.Serialize(request);
            var message = _messageFactory.Create(bodyStream, _serializer.Encoding, messageLabel);

            var msg = _queue.Send(message);
            return msg;
        }

        /// <summary>
        /// Get a <see cref="ProcessingRequest"/> for the specified message
        /// </summary>
        /// <param name="messageId">Unique identifier for the message</param>
        public virtual ProcessingRequest GetProcessingRequest(string messageId) {
            var message = GetMessage(messageId);
            var jobRequest = CreateJobRequest(message);
            var processingRequest = CreateProcessingRequest(jobRequest, message);
            return processingRequest;
        }

        /// <summary>
        /// Receive a message by <see cref="IMessage.Id"/>
        /// </summary>
        /// <param name="messageId">Unique identifier for the message</param>
        public virtual void ReceiveMessage(string messageId) {
            _queue.Receive(messageId);
        }

        /// <summary>
        /// Receive the most recent job on the queue
        /// </summary>
        public virtual void ReceiveMessage() {
            _queue.Receive();
        }

        /// <summary>
        /// Close the connection to the queue and stop listening
        /// </summary>
        public void Close() {
            _queue.EndRecieve();
        }

        /// <summary>
        /// Get all messages from the queue
        /// </summary>
        public IEnumerable<IMessage> GetMessages() {
            return _queue.GetMessages();
        }

        /// <summary>
        /// Get a specific message from the queue
        /// </summary>
        /// <param name="messageId">Unique identifier for the message</param>
        public IMessage GetMessage(string messageId) {
            return _queue.GetMessage(messageId);
        }

        /// <summary>
        /// Delete all messages from the queue
        /// </summary>
        public void Purge() {
            _queue.Purge();
        }

        /// <summary>
        /// Delete a specific message from the queue
        /// </summary>
        /// <param name="messageId">Unique identifier for the message</param>
        public void DeleteMessage(string messageId) {
            _queue.DeleteMessage(messageId);
        }

        #region IDisposable Support

        /// <summary>
        /// Release all resources used by the <see cref="JobQueue"/>
        /// </summary>
        /// <param name="disposing">Whether or not the objects is already being disposed</param>
        protected virtual void Dispose(bool disposing) {
            if (!_disposedValue) {
                if (disposing) {
                    _queue?.Dispose();
                }
                _disposedValue = true;
            }
        }

        /// <summary>
        /// Release all resources used by the <see cref="JobQueue"/>
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }
        #endregion

        /// <summary>
        /// Returns a string that describes the <see cref="JobQueue"/>
        /// </summary>
        public override string ToString() {
            return $"Queue '{_queue.Name}' ({_queue.Id})";
        }
    }
}
