using System.IO;
using System.Text;

namespace AnyQ.Queues.Msmq {
    /// <summary>
    /// Provides methods for creating <see cref="MsmqMessage"/> objects
    /// </summary>
    public class MsmqMessageFactory : IMessageFactory {
        /// <summary>
        /// Creates a new <see cref="MsmqMessage"/> from the specified stream
        /// </summary>
        /// <param name="bodyStream"><see cref="Stream"/> containing the message data</param>
        /// <param name="label">Human-readable label for the message</param>
        public IMessage Create(Stream bodyStream, string label) {
            return new MsmqMessage(bodyStream, label);
        }

        /// <summary>
        /// Creates a new <see cref="MsmqMessage"/> from the specified stream
        /// </summary>
        /// <param name="body">String containing the message data</param>
        /// <param name="label">Human-readable label for the message</param>
        public static IMessage CreateMessage(string body, string label) {
            var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(body));
            return new MsmqMessage(bodyStream, label);
        }

        /// <summary>
        /// Creates a new <see cref="MsmqMessage"/> from the specified stream
        /// </summary>
        /// <param name="bodyStream"><see cref="Stream"/> containing the message data</param>
        /// <param name="label">Human-readable label for the message</param>
        public static IMessage CreateMessage(Stream bodyStream, string label) {
            return new MsmqMessage(bodyStream, label);
        }
    }
}
