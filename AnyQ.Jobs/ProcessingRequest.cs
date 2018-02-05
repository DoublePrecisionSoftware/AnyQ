namespace AnyQ.Jobs {
    /// <summary>
    /// Represents a request for processing a job
    /// </summary>
    public class ProcessingRequest {
        /// <summary>
        /// Unique identifier for the job
        /// </summary>
        public string JobId { get; set; }
        /// <summary>
        /// Human-readable name for the job
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Unique identifier for the queue the request originated from
        /// </summary>
        public string QueueId { get; set; }
        /// <summary>
        /// Human-readable name for the queue the request originated from
        /// </summary>
        public string QueueName { get; set; }
        /// <summary>
        /// Body of the job request
        /// </summary>
        public JobRequest JobRequest { get; set; }

        /// <summary>
        /// Returns a useful string representation of the request
        /// </summary>
        public override string ToString() {
            return 
                $"Request for Job {JobId} ({(string.IsNullOrEmpty(Name) ? "unnamed" : Name)}) on {(string.IsNullOrWhiteSpace(QueueName) ? QueueId :$"{QueueName} ({QueueId})")}";
        }
    }
}
