﻿using System.IO;

namespace AnyQ.Queues {
    /// <summary>
    /// Provides methods for creating <see cref="IMessage"/> instances for use in an <see cref="IMessageQueue"/>
    /// </summary>
    public interface IMessageFactory {
        /// <summary>
        /// Create an <see cref="IMessage"/> with the provided <see cref="Stream"/> for creating the body
        /// </summary>
        /// <param name="bodyStream"><see cref="Stream"/> containing the body</param>
        /// <param name="label">Human-readable name for the message</param>
        IMessage Create(Stream bodyStream, string label);
    }
}