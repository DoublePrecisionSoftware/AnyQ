using AnyQ.Queues;
using System;

namespace AnyQ {
    /// <summary>
    /// Represents even data associated with a <see cref="Jobs.ProcessingRequest"/> being redirected
    /// </summary>
    public class RequestRedirectedEventArgs : EventArgs {
        /// <summary>
        /// Creates a new instance of the <see cref="RequestRedirectedEventArgs"/> class
        /// </summary>
        /// <param name="message">Message generated from redirect</param>
        /// <param name="fromQueue">Id of queue from which the message originated</param>
        /// <param name="toQueue">Id of queue the new message will be sent to</param>
        public RequestRedirectedEventArgs(IMessage message, string fromQueue, string toQueue) {
            NewMessage = message;
            FromQueue = fromQueue;
            ToQueue = toQueue;
        }

        /// <summary>
        /// Message generated upon redirect
        /// </summary>
        public IMessage NewMessage { get; }

        /// <summary>
        /// Message generated upon redirect
        /// </summary>
        public string FromQueue { get; }

        /// <summary>
        /// Message generated upon redirect
        /// </summary>
        public string ToQueue { get; }
    }
}
