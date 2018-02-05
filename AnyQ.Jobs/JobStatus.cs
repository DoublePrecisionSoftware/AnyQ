using System;

namespace AnyQ.Jobs {
    /// <summary>
    /// Represents information about the status of a <see cref="ProcessingRequest"/>
    /// </summary>
    public class JobStatus {
        /// <summary>
        /// Unique identifier for the message containing this job request
        /// </summary>
        public string JobId { get; set; }
        /// <summary>
        /// Human-readable name for the message containing this job request
        /// </summary>
        public string JobName { get; set; }
        /// <summary>
        /// String describing the status of the job
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// String providing more details on the status (e.g., <see cref="Exception"/> information)
        /// </summary>
        public string Details { get; set; }
        /// <summary>
        /// Unique identifier of the originating queue
        /// </summary>
        public string QueueId { get; set; }
        /// <summary>
        /// Descriptive name of the originating queue
        /// </summary>
        public string QueueName { get; set; }
        /// <summary>
        /// Date this object was created
        /// </summary>
        public DateTime CreatedOn => DateTime.UtcNow;

        /// <summary>
        /// Indicates that a queue has received the <see cref="ProcessingRequest"/>
        /// </summary>
        public static readonly string Received = "Received";
        /// <summary>
        /// Indicates that an <see cref="JobHandler"/> is currently processing the <see cref="ProcessingRequest"/>
        /// </summary>
        public static readonly string Processing = "Processing";
        /// <summary>
        /// Indicates that the <see cref="ProcessingRequest"/> was unable to be handled by any currently registered <see cref="JobHandler"/>
        /// </summary>
        public static readonly string Skipped = "Skipped";
        /// <summary>
        /// Indicates that the <see cref="ProcessingRequest"/> was interrupted by a cancellation
        /// </summary>
        public static readonly string Canceled = "Canceled";
        /// <summary>
        /// Indicates that the <see cref="ProcessingRequest"/> was processed successfully
        /// </summary>
        public static readonly string Complete = "Complete";
        /// <summary>
        /// Indicates that processing of the <see cref="ProcessingRequest"/> failed
        /// </summary>
        public static readonly string Failed = "Failed";
        /// <summary>
        /// Indicates that the <see cref="ProcessingRequest"/> has stalled during processing
        /// </summary>
        public static readonly string Stalled = "Stalled";
        /// <summary>
        /// Indicates that the <see cref="ProcessingRequest"/> processing time has exceeded the timeout limit
        /// </summary>
        public static readonly string TimedOut = "TimedOut";
        /// <summary>
        /// Indicates that the <see cref="ProcessingRequest"/> processing time has been redirected to another queue
        /// </summary>
        public static readonly string Redirected = "Redirected";
    }
}
