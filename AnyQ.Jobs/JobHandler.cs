using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AnyQ.Jobs {
    /// <summary>
    /// Provides methods and properties for processing queue messages via <see cref="ProcessingRequest"/> objects
    /// </summary>
    public abstract class JobHandler : IEquatable<JobHandler> {
        
        /// <summary>
        /// The configuration used to create the corresponding queue for this handler
        /// </summary>
        public abstract HandlerConfiguration Configuration { get; }

        /// <summary>
        /// Returns whether or not this handler can process the job described by the <see cref="ProcessingRequest"/>
        /// </summary>
        /// <param name="request">Data describing the job</param>
        public abstract bool CanProcess(ProcessingRequest request);

        /// <summary>
        /// Processes the job
        /// </summary>
        /// <param name="request">Data describing the job</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to provide to any async actions</param>
        public abstract Task ProcessAsync(ProcessingRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Fired when a job is successfully completed
        /// </summary>
        public event EventHandler<ProcessingCompletedEventArgs> ProcessingCompleted;

        /// <summary>
        /// Fired when a job's processing fails
        /// </summary>
        public event EventHandler<ProcessingFailedEventArgs> ProcessingFailed;

        /// <summary>
        /// Provides a list of <see cref="RedirectStrategy"/> objects for resilient processing
        /// </summary>
        public virtual IEnumerable<RedirectStrategy> GetRedirectStrategies() => new RedirectStrategy[0];

        /// <summary>
        /// Invoked when the processing of a <see cref="ProcessingRequest"/> completes successfully
        /// </summary>
        /// <param name="request"><see cref="ProcessingRequest"/> provided to <see cref="ProcessAsync(ProcessingRequest, CancellationToken)"/></param>
        /// <param name="result">Optional result of processing</param>
        protected virtual void OnProcessingCompleted(ProcessingRequest request, string result = null) {
            ProcessingCompleted?.Invoke(this, new ProcessingCompletedEventArgs(request, result));
        }

        /// <summary>
        /// Invoked when the processing of a <see cref="ProcessingRequest"/> fails
        /// </summary>
        /// <param name="request"><see cref="ProcessingRequest"/> provided to <see cref="ProcessAsync(ProcessingRequest, CancellationToken)"/></param>
        /// <param name="exception">Exception generated from failure</param>
        protected virtual void OnProcessingFailed(ProcessingRequest request, Exception exception) {
            ProcessingFailed?.Invoke(this, new ProcessingFailedEventArgs(request, exception));
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="JobHandler"/>
        /// </summary>
        /// <param name="other">A <see cref="JobHandler"/> to compare to this instance</param>
        public bool Equals(JobHandler other) => Equals(other);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object
        /// </summary>
        /// <param name="obj">An object to compare to this instance</param>
        public override bool Equals(object obj) => GetHashCode().Equals(obj.GetHashCode());

        /// <summary>
        /// Returns the hash code for this queue's identifiers
        /// </summary>
        public override int GetHashCode() => Configuration.QueueId.GetHashCode() ^ Configuration.QueueName.GetHashCode();

        /// <summary>
        /// Returns a string that describes the <see cref="JobHandler"/>
        /// </summary>
        public override string ToString() => $"Handler for {Configuration.QueueName} ({Configuration.QueueId})";
    }
}
