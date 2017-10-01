using Newtonsoft.Json;
using Toffee.Server.Internal;

namespace Toffee.Server.Roles.ClientAgent
{
    public class ClientAgentConfiguration : ToffeeServerConfiguration
    {
        [JsonProperty("director", Required = Required.Always)]
        public ToffeeInternalConfiguration Internal { get; private set; }

        [JsonProperty("app", Required = Required.Always)]
        public ApplicationConfiguration Application { get; private set; }

        [JsonProperty("heartbeat")]
        public byte HeartbeatTime { get; private set; }

        public ClientAgentConfiguration(int port, ToffeeNetworkConfiguration network,
            uint maximumConnections = 0, bool forceEncryption = false, ulong encryptionKey = 0)
            : base (port, network, maximumConnections, forceEncryption, encryptionKey)
        {
        }
    }
}
