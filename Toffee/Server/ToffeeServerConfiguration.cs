using Newtonsoft.Json;

namespace Toffee.Server
{
    public class ToffeeServerConfiguration
    {
        /// <summary>
        /// The port the server should listen on.
        /// </summary>
        [JsonProperty("port", Required = Required.Always)]
        public int Port { get; private set; }
        
        /// <summary>
        /// The network settings for this server.
        /// </summary>
        [JsonProperty("network")]
        public ToffeeNetworkConfiguration Network { get; private set; }

        /// <summary>
        /// The maximum number of connections the server should able to have connected at once.
        /// If this value is zero, then there will be no limits.
        /// </summary>
        [JsonProperty("max_connections")]
        public uint MaximumConnections { get; private set; }

        /// <summary>
        /// True if the client/server expects all packets to be encrypted, and false otherwise.
        /// </summary>
        [JsonProperty("force_encryption")]
        public bool ForceEncryption { get; private set; }

        /// <summary>
        /// The encryption key.
        /// </summary>
        [JsonProperty("encryption_key")]
        public ulong EncryptionKey { get; private set; }

        public ToffeeServerConfiguration(int port, ToffeeNetworkConfiguration network,
            uint maximumConnections = 0, bool forceEncryption = false, ulong encryptionKey = 0)
        {
            Port = port;
            Network = network;
            MaximumConnections = maximumConnections;
            ForceEncryption = forceEncryption;
            EncryptionKey = encryptionKey;
        }
    }
}
