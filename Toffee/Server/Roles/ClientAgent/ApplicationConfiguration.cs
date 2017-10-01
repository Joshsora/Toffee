using Newtonsoft.Json;

namespace Toffee.Server.Roles.ClientAgent
{
    public class ApplicationConfiguration
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; private set; }

        [JsonProperty("version", Required = Required.Always)]
        public string Version { get; private set; }
    }
}
