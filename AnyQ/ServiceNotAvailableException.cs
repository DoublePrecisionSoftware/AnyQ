using System;

namespace AnyQ {

    /// <summary>
    /// Exception indicating that a queuing service is not available
    /// </summary>
    [Serializable]
    public class ServiceNotAvailableException : Exception {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceNotAvailableException"/> class
        /// </summary>
        public ServiceNotAvailableException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceNotAvailableException"/> class
        /// </summary>
        /// <param name="message">Message to set in exception</param>
        public ServiceNotAvailableException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceNotAvailableException"/> class
        /// </summary>
        /// <param name="message">Message to set in exception</param>
        /// <param name="inner">Inner exception</param>
        public ServiceNotAvailableException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceNotAvailableException"/> class
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ServiceNotAvailableException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
