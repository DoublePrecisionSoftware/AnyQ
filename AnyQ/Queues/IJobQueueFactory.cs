namespace AnyQ.Queues {
    /// <summary>
    /// Provides methods for creating a <see cref="JobQueue"/>
    /// </summary>
    public interface IJobQueueFactory {
        /// <summary>
        /// Retrieve an instance of an existing queue
        /// </summary>
        /// <param name="queueId">Unique Id for an existing queue</param>
        JobQueue Create(string queueId);
        /// <summary>
        /// Create a new <see cref="JobQueue"/> for the specified queue
        /// </summary>
        /// <param name="handlerConfiguration">Configuration for handler</param>
        JobQueue Create(Jobs.HandlerConfiguration handlerConfiguration);
    }
}
