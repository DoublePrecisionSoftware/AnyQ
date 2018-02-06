namespace AnyQ {

    /// <summary>
    /// Provides access to common configuration values related to a <see cref="JobQueueListener"/> 
    /// </summary>
    public class ListenerConfiguration {
        /// <summary>
        /// Amount of time in milliseconds before a job is canceled and the status is reported as "TimedOut"
        /// </summary>
        public int JobTimeout { get; set; }

        /// <summary>
        /// Prefix to append to all queue identifiers
        /// </summary>
        public string QueuePrefix { get; set; }
    }
}
