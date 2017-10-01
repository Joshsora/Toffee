using Newtonsoft.Json;
using Toffee.Server.Roles;
using Toffee.Logging;

namespace ToffeeDaemon
{
    public class ToffeeDaemonConfiguration
    {
        [JsonProperty("debug")]
        public bool Debug { get; private set; }

        [JsonProperty("tick-rate")]
        public int TickRate { get; private set; } = 1000;

        [JsonProperty("assemblies", Required = Required.Always)]
        public string[] Assemblies { get; private set; }

        [JsonProperty("loggers")]
        public LoggerConfiguration[] Loggers { get; private set; }

        [JsonProperty("roles", Required = Required.Always)]
        public ToffeeRoleConfiguration[] Roles { get; private set; }
    }
}
