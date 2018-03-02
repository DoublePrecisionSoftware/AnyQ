namespace AnyQ.Formatters {
    /// <summary>
    /// Provides methods for reading and writing payload data to and from the serialized <see cref="string"/>
    /// </summary>
    public interface IPayloadFormatter {
        /// <summary>
        /// Write the data to a <see cref="string"/>
        /// </summary>
        /// <param name="payload">Object containing the payload data</param>
        byte[] Write(object payload);
        /// <summary>
        /// Deserialize the payload string into an instance of <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of data represented in the payload</typeparam>
        /// <param name="payloadString"><see cref="string"/> containing the serialized payload data</param>
        T Read<T>(string payloadString) where T : class;
    }
}
