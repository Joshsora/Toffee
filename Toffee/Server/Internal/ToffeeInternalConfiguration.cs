using Newtonsoft.Json;

namespace Toffee.Server.Internal
{
    public class ToffeeInternalConfiguration
    {
        [JsonProperty("host", Required = Required.Always)]
        public string Host { get; private set; }

        [JsonProperty("port", Required = Required.Always)]
        public int Port { get; private set; }

        [JsonProperty("identity", Required = Required.Always)]
        public ToffeeInternalIdentity Identity { get; private set; }

        [JsonProperty("channelCount")]
        public uint ChannelCount { get; set; }
    }
}
