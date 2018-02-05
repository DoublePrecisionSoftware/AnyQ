using System;

namespace AnyQ.Queues {
    /// <summary>
    /// Defines options for creating <see cref="IMessageQueue"/> objects
    /// </summary>
    public class QueueCreationOptions {
        /// <summary>
        /// Unique Id for the <see cref="IMessageQueue"/>
        /// </summary>
        public string QueueId { get; set; }
        /// <summary>
        /// Human-readable name for the <see cref="IMessageQueue"/>
        /// </summary>
        public string QueueName { get; set; }
        /// <summary>
        /// Rate at which the queue receives jobs
        /// </summary>
        public TimeSpan? ThrottleInterval { get; set; }
    }
}
