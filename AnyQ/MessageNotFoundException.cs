using System;

namespace AnyQ {

    /// <summary>
    /// Represents an exception indicating that a message cannot be found.
    /// </summary>
    [Serializable]
    public class MessageNotFoundException : Exception {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageNotFoundException"/> class
        /// </summary>
        public MessageNotFoundException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageNotFoundException"/> class
        /// </summary>
        /// <param name="message">Message of exception</param>
        public MessageNotFoundException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageNotFoundException"/> class
        /// </summary>
        /// <param name="message">Message of exception</param>
        /// <param name="inner">Exception that caused this exception</param>
        public MessageNotFoundException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageNotFoundException"/> class
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected MessageNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
