using System;

namespace AnyQ {

    /// <summary>
    /// Represents an exception indicating that a queue cannot be accessed.
    /// </summary>
    [Serializable]
    public class QueueNotAvailableException : Exception {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueNotAvailableException"/> class
        /// </summary>
        public QueueNotAvailableException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueNotAvailableException"/> class
        /// </summary>
        /// <param name="message">Message of exception</param>
        public QueueNotAvailableException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueNotAvailableException"/> class
        /// </summary>
        /// <param name="message">Message of exception</param>
        /// <param name="inner">Inner exception</param>
        public QueueNotAvailableException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueNotAvailableException"/> class
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected QueueNotAvailableException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
