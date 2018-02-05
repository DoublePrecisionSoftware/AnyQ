using AnyQ.Jobs;
using System.IO;

namespace AnyQ.Queues {
    /// <summary>
    /// Provides mechanisms for converting a <see cref="JobRequest"/> to and from a <see cref="Stream"/> for use in an <see cref="IMessage.BodyStream"/>
    /// </summary>
    public interface IRequestSerializer {
        /// <summary>
        /// Serialize the <see cref="JobRequest"/> into a <see cref="Stream"/>
        /// </summary>
        /// <param name="request">Job request</param>
        Stream Serialize(JobRequest request);
        /// <summary>
        /// Deserialize a <see cref="Stream"/> from an <see cref="IMessage.BodyStream"/> into a <see cref="JobRequest"/>
        /// </summary>
        /// <param name="request"><see cref="Stream"/> from an <see cref="IMessage.BodyStream"/></param>
        JobRequest Deserialize(Stream request);
    }
}
