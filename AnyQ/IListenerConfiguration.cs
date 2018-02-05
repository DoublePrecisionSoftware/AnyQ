namespace AnyQ {

    /// <summary>
    /// Provides access to common configuration values related to a <see cref="JobQueueListener"/> 
    /// </summary>
    public interface IListenerConfiguration {
        /// <summary>
        /// Amount of time in milliseconds before a job is canceled and the status is reported as "TimedOut"
        /// </summary>
        int JobTimeout { get; }

        /// <summary>
        /// Prefix to append to all queue identifiers
        /// </summary>
        string QueuePrefix { get;  }
    }
}
