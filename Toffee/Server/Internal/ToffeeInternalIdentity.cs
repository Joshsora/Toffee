using Newtonsoft.Json;

namespace Toffee.Server.Internal
{
    public class ToffeeInternalIdentity
    {
        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("channel")]
        public uint Channel { get; private set; }
    }
}
