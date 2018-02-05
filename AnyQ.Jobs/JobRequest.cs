namespace AnyQ.Jobs {
    /// <summary>
    /// Represents a job to be performed
    /// </summary>
    public class JobRequest {
        /// <summary>
        /// Name for the type of work to perform
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Serialized data for use by the job handler
        /// </summary>
        public string Payload { get; set; }

        /// <summary>
        /// Returns a simple description of the job
        /// </summary>
        public override string ToString() {
            return $"Job '{Type}'";
        }
    }
}
