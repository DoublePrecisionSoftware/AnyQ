using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Messaging;
using System.Threading;

namespace AnyQ.Queues.Msmq {
    /// <summary>
    /// Represents an MSMQ queue
    /// </summary>
    public class MsmqMessageQueue : IMessageQueue {

        private MessageQueue _queue;
        private readonly QueueCreationOptions _options;
        private readonly AccessControlList _accessControl;
        private Timer _throttleTimer;
        private bool _listening = false;
        private static readonly TimeSpan RecieveWaitTimeout = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="MsmqMessageQueue"/> class
        /// </summary>
        /// <param name="options">Options for creating the queue</param>
        /// <param name="transactional">Pass true to create a transactional queue</param>
        /// <param name="accessControl">List of accounts that have specific control over the queue</param>
        public MsmqMessageQueue(QueueCreationOptions options, bool transactional, AccessControlList accessControl = null) {

            // TODO: deal with Priority? https://msdn.microsoft.com/en-us/library/system.messaging.message.priority(v=vs.110).aspx

            _options = options ?? throw new ArgumentNullException(nameof(options));
            _accessControl = accessControl;
            InitializeQueue(options.QueueId, transactional);
        }

        private void InitializeQueue(string path, bool transactional) {
            if (!MessageQueue.Exists(path)) {
                _queue = MessageQueue.Create(path, transactional);
                _queue.UseJournalQueue = true;
                if (!string.IsNullOrWhiteSpace(_options.QueueName)) {
                    _queue.Label = _options.QueueName;
                }
            } else {
                _queue = new MessageQueue(path);
                _options.QueueName = _queue.Label;
            }
            if (_accessControl != null) {
                _queue.SetPermissions(_accessControl);
            }
            _queue.ReceiveCompleted += OnReceivedMessage;
        }

        private void OnReceivedMessage(object sender, Message message) {
            ReceivedMessage?.Invoke(sender, new ReceivedMessageEventArgs(new MsmqMessage(message)));
            BeginReceive();
        }

        private void OnReceivedMessage(object sender, ReceiveCompletedEventArgs e) {
            OnReceivedMessage(sender, e.Message);
        }

        /// <summary>
        /// Unique identifier for the queue
        /// </summary>
        public string Id => _options.QueueId;

        /// <summary>
        /// Label of the queue
        /// </summary>
        public string Name => _options.QueueName;

        /// <summary>
        /// Raised when a message arrives on the queue
        /// </summary>
        public event EventHandler<ReceivedMessageEventArgs> ReceivedMessage;

        /// <summary>
        /// Begins listening to the queue
        /// </summary>
        public void BeginReceive() {
            _listening = true;
            if (_options.ThrottleInterval != null) {
                _throttleTimer = _throttleTimer ?? new Timer(state => {
                    TimerCallback();
                }, null, TimeSpan.Zero, _options.ThrottleInterval.Value);
            } else {
                _queue.BeginReceive();
            }
        }

        /// <summary>
        /// Receives a message by Id and fires <see cref="ReceivedMessage"/>
        /// </summary>
        /// <param name="messageId">Unique Id of message to receive</param>
        /// <exception cref="MessageQueueException" />
        public void Receive(string messageId) {
            var msg = _queue.ReceiveById(messageId, RecieveWaitTimeout);
            OnReceivedMessage(_queue, msg);
        }

        /// <summary>
        /// Receives the next message on the queue and fires <see cref="ReceivedMessage"/>
        /// </summary>
        /// <exception cref="MessageQueueException" />
        public void Receive() {
            var msg = _queue.Receive(RecieveWaitTimeout);
            OnReceivedMessage(_queue, msg);
        }

        private void TimerCallback() {
            if (!_listening) {
                _throttleTimer.Dispose();
                _throttleTimer = null;
                return;
            }
            try {
                var message = _queue.Receive(RecieveWaitTimeout);
                OnReceivedMessage(_queue, message);
            } catch (MessageQueueException ex) {
                Trace.TraceError($"Error receiving message on queue {_queue.Id}.{Environment.NewLine}{ex}");
            }
        }

        /// <summary>
        /// Closes the queue
        /// </summary>
        public void EndRecieve() {
            _queue.Close();
        }

        /// <summary>
        /// Add the specified message to the queue
        /// </summary>
        /// <param name="message">Message to send</param>
        public IMessage Send(IMessage message) {
            var msg = ((MsmqMessage)message).ToMessage();
            _queue.Send(msg);
            return new MsmqMessage(msg);
        }

        /// <summary>
        /// Returns all messages from the queue
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMessage> GetMessages() {
            return _queue.GetAllMessages().Select(m => new MsmqMessage(m));
        }

        /// <summary>
        /// Returns a specific message from the queue
        /// </summary>
        /// <param name="messageId">Unique Id of message</param>
        public IMessage GetMessage(string messageId) {
            try {
                return GetMessages().SingleOrDefault(m => m.Id == messageId);
            } catch (Exception ex) {
                throw new MessageNotFoundException($"Could not find message with Id {messageId}", ex);
            }
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
        /// <param name="messageId"></param>
        /// <exception cref="MessageNotFoundException">Thrown if the message at id <paramref name="messageId"/> cannot be found</exception>
        public void DeleteMessage(string messageId) {
            try {
                _queue.ReceiveById(messageId, TimeSpan.FromSeconds(1)); // Receive, but do not process
            } catch (MessageQueueException ex) {
                throw new MessageNotFoundException($"Could not find message with Id {messageId}", ex);
            }
        }

        #region IDisposable Support
        private bool _disposedValue = false;

        /// <summary>
        /// Release all resources used by the <see cref="MsmqMessageQueue"/>
        /// </summary>
        /// <param name="disposing">True if the object is already being disposed</param>
        protected virtual void Dispose(bool disposing) {
            if (!_disposedValue) {
                if (disposing) {
                    _queue?.Dispose();
                }
                _disposedValue = true;
            }
        }

        /// <summary>
        /// Release all resources used by the <see cref="MsmqMessageQueue"/>
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }

        #endregion
    }
}
