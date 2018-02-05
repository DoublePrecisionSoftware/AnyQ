using System;
using System.Collections.Generic;

namespace AnyQ.Queues {
    /// <summary>
    /// Represents a queue that sends and receives <see cref="IMessage"/>s
    /// </summary>
    public interface IMessageQueue : IDisposable {
        /// <summary>
        /// Unique identifier for the queue
        /// </summary>
        string Id { get; }
        /// <summary>
        /// Human-readable name for the queue
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Push a message onto the queue
        /// </summary>
        /// <param name="message"></param>
        IMessage Send(IMessage message);
        /// <summary>
        /// Fired when an <see cref="IMessage"/> is received
        /// </summary>
        event EventHandler<ReceivedMessageEventArgs> ReceivedMessage;
        /// <summary>
        /// Begin listening to the queue
        /// </summary>
        void BeginReceive();
        /// <summary>
        /// Receive a message by unique ID
        /// </summary>
        void Receive(string messageId);
        /// <summary>
        /// Receive the message on top of the queue
        /// </summary>
        void Receive();
        /// <summary>
        /// Stop listening and close the queue
        /// </summary>
        void EndRecieve();
        /// <summary>
        /// Get all messages from the queue
        /// </summary>
        IEnumerable<IMessage> GetMessages();
        /// <summary>
        /// Get a specific message from the queue
        /// </summary>
        /// <param name="messageId">Unique identifier for the message</param>
        IMessage GetMessage(string messageId);
        /// <summary>
        /// Delete all messages from the queue
        /// </summary>
        void Purge();
        /// <summary>
        /// Delete a specific message from the queue
        /// </summary>
        /// <param name="messageId">Unique identifier for the message</param>
        void DeleteMessage(string messageId);
    }
}
