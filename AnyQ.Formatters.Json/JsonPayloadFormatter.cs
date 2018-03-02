using Newtonsoft.Json;
using System;
using System.Text;

namespace AnyQ.Formatters {
    /// <summary>
    /// Formats payloads in JSON format
    /// </summary>
    public class JsonPayloadFormatter : IPayloadFormatter {
        private readonly Encoding _encoding;

        public JsonPayloadFormatter(Encoding encoding = null) {
            _encoding = encoding ?? Encoding.UTF8;
        }

        /// <summary>
        /// Deserialize the payload string into an instance of <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of data represented in the payload</typeparam>
        /// <param name="payloadString"><see cref="string"/> containing the serialized payload data</param>
        public T Read<T>(byte[] payload) where T : class {
            if (payload == null) {
                throw new ArgumentNullException(nameof(payload));
            }

            var payloadString = _encoding.GetString(payload);

            if (string.IsNullOrWhiteSpace(payloadString)) {
                return null;
            }

            return JsonConvert.DeserializeObject<T>(payloadString);
        }

        /// <summary>
        /// Write the data to a <see cref="string"/>
        /// </summary>
        /// <param name="payload">Object containing the payload data</param>
        public byte[] Write(object payload) {
            if (payload == null) {
                return null;
            }
            if (payload is string) {
                var payloadString = payload as string;
                if (!string.IsNullOrWhiteSpace(payloadString)) {
                    return _encoding.GetBytes(payload as string);
                }
            }
            var jsonString = JsonConvert.SerializeObject(payload);
            var unescaped = Uri.UnescapeDataString(jsonString);
            return _encoding.GetBytes(unescaped);
        }
    }
}
