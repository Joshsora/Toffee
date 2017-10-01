using Newtonsoft.Json;

namespace Toffee.Server
{
    public class ToffeeNetworkConfiguration
    {
        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("namespace")]
        public string Namespace { get; private set; }

        [JsonProperty("suffix")]
        public string Suffix { get; private set; }

        [JsonProperty("hash")]
        public uint Hash { get; private set; }

        [JsonProperty("files")]
        public string[] Files { get; private set; }

        public ToffeeNetworkConfiguration(string name)
        {
            Name = name;
        }
    }
}
