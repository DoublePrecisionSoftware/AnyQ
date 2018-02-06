using AnyQ.Jobs;
using System;

namespace AnyQ.Queues {

    /// <summary>
    /// Represents event data for receiving a job on a <see cref="JobQueue"/>
    /// </summary>
    public class JobReceivedEventArgs : EventArgs {
        /// <summary>
        /// Initialize a new instance of the <see cref="JobReceivedEventArgs"/> class
        /// </summary>
        /// <param name="request">Originating request</param>
        public JobReceivedEventArgs(ProcessingRequest request) {
            ProcessingRequest = request;
        }

        /// <summary>
        /// Request generated from the received <see cref="IMessage"/>
        /// </summary>
        public ProcessingRequest ProcessingRequest { get; }
    }
}
