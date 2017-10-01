using Newtonsoft.Json;

namespace Toffee.Logging
{
    public class LoggerConfiguration
    {
        /// <summary>
        /// The kind of logger to create.
        /// </summary>
        [JsonProperty("kind", Required = Required.Always)]
        public string Kind { get; private set; }

        /// <summary>
        /// The logger's configuration.
        /// </summary>
        [JsonProperty("config")]
        public dynamic Configuration { get; private set; }
    }
}
