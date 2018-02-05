using AnyQ.Jobs;

namespace AnyQ.Queues.Status {
    /// <summary>
    /// Provides methods for writing <see cref="JobStatus"/> entries to a store
    /// </summary>
    public interface IStatusProvider {
        /// <summary>
        /// Write the provided status to the store
        /// </summary>
        /// <param name="status">The <see cref="JobStatus"/> to write to the store</param>
        void WriteStatus(JobStatus status);
        /*x
        /// <summary>
        /// Retrieve the status of a job from the store
        /// </summary>
        /// <param name="jobId">Unique identifier for the job</param>
        JobStatus GetStatus(string jobId);
        */
    }
}
