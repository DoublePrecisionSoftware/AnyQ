using System.Text;

namespace AnyQ.Queues {
    /// <summary>
    /// Provides methods for creating <see cref="IMessage"/> instances for use in an <see cref="IMessageQueue"/>
    /// </summary>
    public interface IMessageFactory {
        /// <summary>
        /// Create an <see cref="IMessage"/> with the provided bytes for creating the body
        /// </summary>
        /// <param name="body">Byte array making up the body of the message</param>
        /// <param name="label">Human-readable name for the message</param>
        IMessage Create(byte[] body, string label);
    }
}
