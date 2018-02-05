using System;

namespace AnyQ.Jobs {
    /// <summary>
    /// Represents the event data for when an <see cref="JobHandler"/> completes processing a <see cref="ProcessingRequest"/>
    /// </summary>
    public class ProcessingCompletedEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingCompletedEventArgs"/> class
        /// </summary>
        /// <param name="request">Originating request</param>
        /// <param name="resultBody">Optional result of processing</param>
        public ProcessingCompletedEventArgs(ProcessingRequest request, string resultBody = null) {
            ProcessingRequest = request;
            ResultBody = resultBody;
        }

        /// <summary>
        /// <see cref="ProcessingRequest"/> that was successfully processed
        /// </summary>
        public ProcessingRequest ProcessingRequest { get; }
        /// <summary>
        /// Serialized body of result of processing
        /// </summary>
        public string ResultBody { get; }
    }

    /// <summary>
    /// Represents the event data for when processing of a <see cref="ProcessingRequest"/> fails
    /// </summary>
    public class ProcessingFailedEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingCompletedEventArgs"/> class
        /// </summary>
        /// <param name="request">Originating request</param>
        /// <param name="ex">Exceptions generated from failure</param>
        public ProcessingFailedEventArgs(ProcessingRequest request, Exception ex) {
            ProcessingRequest = request;
            Exception = ex;
        }

        /// <summary>
        /// <see cref="ProcessingRequest"/> that was processed
        /// </summary>
        public ProcessingRequest ProcessingRequest { get; }
        /// <summary>
        /// Set of exceptions that occurred.
        /// </summary>
        public Exception Exception { get; }
    }
}
