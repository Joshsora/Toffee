using Newtonsoft.Json;

namespace Toffee.Server.Roles
{
    public class ToffeeRoleConfiguration
    {
        [JsonProperty("role", Required = Required.Always)]
        public string Name { get; private set; }

        [JsonProperty("config", Required = Required.Always)]
        public dynamic Config { get; private set; }
    }
}
