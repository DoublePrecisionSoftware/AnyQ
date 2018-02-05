using System;

namespace AnyQ.Jobs {
    /// <summary>
    /// Defines options for creating the queue assigned to a <see cref="JobHandler"/>
    /// </summary>
    public class HandlerConfiguration {
        /// <summary>
        /// Unique Id for the queue
        /// </summary>
        public string QueueId { get; set; }
        /// <summary>
        /// Human-readable name for the queue
        /// </summary>
        public string QueueName { get; set; }
        /// <summary>
        /// Rate at which the queue receives jobs
        /// </summary>
        public TimeSpan? ThrottleInterval { get; set; }
    }
}
