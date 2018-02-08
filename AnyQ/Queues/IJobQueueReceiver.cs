using System;

namespace AnyQ.Queues {
    /// <summary>
    /// Provides methods for receiving messages on a queue and producing <see cref="Jobs.ProcessingRequest"/>s
    /// </summary>
    public interface IJobQueueReceiver {
        /// <summary>
        /// Fired when a <see cref="Jobs.ProcessingRequest"/> is generated from an <see cref="IMessage"/>
        /// </summary>
        event EventHandler<JobReceivedEventArgs> JobReceived;

        /// <summary>
        /// Start listening for jobs on the queue
        /// </summary>
        void StartListening();
        /// <summary>
        /// Stop listening and close the queue
        /// </summary>
        void Close();
    }
}
