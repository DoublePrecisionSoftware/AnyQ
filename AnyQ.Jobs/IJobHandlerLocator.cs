using System.Collections.Generic;

namespace AnyQ.Jobs {
    /// <summary>
    /// Provides methods for locating <see cref="JobHandler"/>s within an assembly
    /// </summary>
    public interface IJobHandlerLocator {
        /// <summary>
        /// Retrieve a <see cref="JobHandler"/> instance by the Id of the queue it listens for
        /// </summary>
        /// <param name="queueId">Unique Id of queue</param>
        /// <param name="handler"><see cref="JobHandler"/> instance found</param>
        /// <returns>Returns true if the <see cref="JobHandler"/> is found</returns>
        bool TryGetHandlerByQueueId(string queueId, out JobHandler handler);
        /// <summary>
        /// Returns all handlers available to this instance
        /// </summary>
        IEnumerable<JobHandler> GetHandlers();
    }
}
