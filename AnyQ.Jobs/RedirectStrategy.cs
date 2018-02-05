using System;

namespace AnyQ.Jobs {
    /// <summary>
    /// Represents a strategy for redirecting a <see cref="ProcessingRequest"/> in the pipeline
    /// </summary>
    public sealed class RedirectStrategy {

        private RedirectStrategy() { }

        /// <summary>
        /// Create a new <see cref="RedirectStrategy"/> with the specified behaviors
        /// </summary>
        /// <param name="decider">Function that returns true if the redirect is to occur</param>
        /// <param name="redirect">Function to return a new queue Id</param>
        public static RedirectStrategy Create(Func<JobStatus, Exception, bool> decider, Func<JobStatus, string> redirect) {
            return new RedirectStrategy {
                Decider = decider,
                Redirect = redirect,
            };
        }
        
        /// <summary>
        /// Function to determine which queue to redirect to
        /// </summary>
        public Func<JobStatus, string> Redirect { get; private set; }
        /// <summary>
        /// Function to determine whether or not to redirect to a new queue
        /// </summary>
        public Func<JobStatus, Exception, bool> Decider { get; private set; }
    }
}
