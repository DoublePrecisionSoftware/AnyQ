namespace AnyQ.Queues {
    /// <summary>
    /// Provides methods for sending jobs to an <see cref="IMessageQueue"/>
    /// </summary>
    public interface IJobQueueSender {
        /// <summary>
        /// Send a message to the queue
        /// </summary>
        /// <param name="type">Type of job to perform</param>
        /// <param name="payload">Payload to pass to the <see cref="Jobs.JobHandler"/></param>
        /// <param name="messageLabel">Label for the message</param>
        /// <returns>Returns the <see cref="IMessage"/> generated from the send request</returns>
        IMessage SendJob(string type, object payload, string messageLabel);
    }
}
