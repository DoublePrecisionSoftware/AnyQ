using System.IO;

namespace AnyQ.Queues {

    /// <summary>
    /// Describes the content of a message
    /// </summary>
    public interface IMessage {
        /// <summary>
        /// Unique identifier for the message
        /// </summary>
        string Id { get; }
        /// <summary>
        /// Human-readable name for the message
        /// </summary>
        string Label { get; }
        /// <summary>
        /// <see cref="Stream"/> containing the message data (a JobRequest object)
        /// </summary>
        byte[] Body { get; }
    }
}
