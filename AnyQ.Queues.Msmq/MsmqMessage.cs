using System;
using System.IO;
using System.Messaging;

namespace AnyQ.Queues.Msmq {
    /// <summary>
    /// Represents an MSMQ message
    /// </summary>
    public class MsmqMessage : IMessage {

        private readonly Message _message;
        
        /// <summary>
        /// Initialize a new instance of the <see cref="MsmqMessage"/> with the data <see cref="Stream"/> and label
        /// </summary>
        /// <param name="bodyStream"><see cref="Stream"/> containing the message data</param>
        /// <param name="label">Human-readable label for the message</param>
        /// <exception cref="ArgumentNullException" />
        public MsmqMessage(Stream bodyStream, string label) {
            if (bodyStream == null) {
                throw new ArgumentNullException(nameof(bodyStream));
            }

            _message = new Message() {
                BodyStream = bodyStream,
                Label = label
            };
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="MsmqMessage"/> class from a <see cref="Message"/>
        /// </summary>
        /// <param name="msg">MSMQ message with relevant data</param>
        /// <exception cref="ArgumentNullException" />
        public MsmqMessage(Message msg) {
            _message = msg ?? throw new ArgumentNullException(nameof(msg));
        }

        /// <summary>
        /// Unique identifier for this message
        /// </summary>
        public string Id => _message.Id;

        /// <summary>
        /// Human-readable label for the message
        /// </summary>
        public string Label => _message.Label;

        /// <summary>
        /// <see cref="Stream"/> containing the message data
        /// </summary>
        public Stream BodyStream => _message.BodyStream;

        /// <summary>
        /// Create a new <see cref="Message"/> instance from this object
        /// </summary>
        internal Message ToMessage() {
            var message = new Message {
                BodyStream = BodyStream,
                Label = Label
            };
            return message;
        }
    }
}
