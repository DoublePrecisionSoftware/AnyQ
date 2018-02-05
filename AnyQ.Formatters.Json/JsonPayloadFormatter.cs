using Newtonsoft.Json;
using System;

namespace AnyQ.Formatters {
    /// <summary>
    /// Formats payloads in JSON format
    /// </summary>
    public class JsonPayloadFormatter : IPayloadFormatter {
        /// <summary>
        /// Deserialize the payload string into an instance of <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of data represented in the payload</typeparam>
        /// <param name="payloadString"><see cref="string"/> containing the serialized payload data</param>
        public T Read<T>(string payloadString) where T : class {
            if (string.IsNullOrWhiteSpace(payloadString)) {
                return null;
            }

            return JsonConvert.DeserializeObject<T>(payloadString);
        }

        /// <summary>
        /// Write the data to a <see cref="string"/>
        /// </summary>
        /// <param name="payloadString">Object containing the payload data</param>
        public string Write(object payloadString) {
            if (payloadString == null) {
                return null;
            }
            if (payloadString is string) {
                return (string)payloadString;
            }
            var jsonString = JsonConvert.SerializeObject(payloadString);
            var unescaped = Uri.UnescapeDataString(jsonString);
            return unescaped;
        }
    }
}
