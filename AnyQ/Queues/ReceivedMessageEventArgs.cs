using System;

namespace AnyQ.Queues {
    /// <summary>
    /// Represents event data for an <see cref="IMessageQueue"/> receiving a message
    /// </summary>
    public class ReceivedMessageEventArgs: EventArgs {

        /// <summary>
        /// Creates a new instance of the <see cref="ReceivedMessageEventArgs"/> class
        /// </summary>
        /// <param name="message">Message received from queue</param>
        public ReceivedMessageEventArgs(IMessage message) {
            Message = message;
        }

        /// <summary>
        /// Message received from queue
        /// </summary>
        public IMessage Message { get; set; }
    }
}
