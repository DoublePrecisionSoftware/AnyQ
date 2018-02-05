using AnyQ.Jobs;
using System;

namespace AnyQ {
    /// <summary>
    /// Represents the data related to the recording of a <see cref="JobStatus"/>
    /// </summary>
    public class StatusReportedEventArgs : EventArgs {

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusReportedEventArgs"/> class
        /// </summary>
        /// <param name="status"><see cref="JobStatus"/> being recorded</param>
        public StatusReportedEventArgs(JobStatus status) {
            Status = status;
        }

        /// <summary>
        /// <see cref="JobStatus"/> being recorded
        /// </summary>
        public JobStatus Status { get; }
    }
}
