using System;

namespace AnyQ.Queues {
    /// <summary>
    /// Provides methods for generating instances of <see cref="IMessageQueue"/>
    /// </summary>
    public interface IMessageQueueFactory {
        /// <summary>
        /// Create an <see cref="IMessageQueue"/> with the specified unique Id and human-readable name
        /// </summary>
        /// <param name="options">Options for creating the queue</param>
        IMessageQueue Create(QueueCreationOptions options);
        /// <summary>
        /// Check if the a queue with the specified unique identifier exists
        /// </summary>
        /// <param name="queueId">Unique identifier for the queue</param>
        bool Exists(string queueId);
    }
}
