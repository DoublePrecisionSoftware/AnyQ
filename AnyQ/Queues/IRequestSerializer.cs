using AnyQ.Jobs;
using System.IO;
using System.Text;

namespace AnyQ.Queues {
    /// <summary>
    /// Provides mechanisms for converting a <see cref="JobRequest"/> to and from a <see cref="Stream"/> for use in an <see cref="IMessage.Body"/>
    /// </summary>
    public interface IRequestSerializer {
        /// <summary>
        /// Serialize the <see cref="JobRequest"/> into a <see cref="Stream"/>
        /// </summary>
        /// <param name="request">Job request</param>
        byte[] Serialize(JobRequest request);
        /// <summary>
        /// Deserialize a <see cref="Stream"/> from an <see cref="IMessage.Body"/> into a <see cref="JobRequest"/>
        /// </summary>
        /// <param name="request"><see cref="byte[]"/> from an <see cref="IMessage.Body"/></param>
        JobRequest Deserialize(byte[] request);

        Encoding Encoding { get; }
    }
}
