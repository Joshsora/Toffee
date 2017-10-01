using Newtonsoft.Json;
using Toffee.Server.Internal;

namespace Toffee.Server.Roles.MessageDirector
{
    public class MessageDirectorConfiguration : ToffeeServerConfiguration
    {
        [JsonProperty("director")]
        public ToffeeInternalConfiguration Internal { get; private set; }

        public MessageDirectorConfiguration(int port, ToffeeNetworkConfiguration network,
    uint maximumConnections = 0, bool forceEncryption = false, ulong encryptionKey = 0)
            : base (port, network, maximumConnections, forceEncryption, encryptionKey)
        {
        }
    }
}
