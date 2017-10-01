using System.Net.Sockets;
using Toffee.Core;
using Toffee.Core.Definitions;
using Toffee.Core.Packets.Internal;
using Toffee.Logging;

namespace Toffee.Server.Internal
{
    /// <summary>
    /// A ToffeeParticipant that implements the internal protocol.
    /// </summary>
    public partial class ToffeeInternalClient : ToffeeParticipant
    {
        /// <summary>
        /// This client's logging category.
        /// </summary>
        private LoggerCategory Log { get; set; }

        /// <summary>
        /// The configuration that was used to set this client up.
        /// </summary>
        private ToffeeInternalConfiguration Configuration { get; set; }

        /// <summary>
        /// This client's role in the internal network.
        /// </summary>
        private string Role { get; set; }

        /// <summary>
        /// Creates a ToffeeInternalClient instance.
        /// </summary>
        public ToffeeInternalClient(ToffeeInternalConfiguration configuration, string roleName = "", LoggerCategory category = null)
            : base(ToffeeNetwork.StandardToffeeNetwork, 0)
        {
            // Properties
            Configuration = configuration;
            Role = roleName;
            Log = category;

            // If we can, log that this instance was created.
            Log?.Info("Created internal client.");

            // Connect to the message director
            Connect(configuration.Host, configuration.Port);
        }

        /// <summary>
        /// Called when a connection attempt is successful.
        /// </summary>
        protected override void ConnectionEstablished()
        {
            // Debug information
            Log?.Debug("Connection established with Message Director. Sending InternalHello.");
            Log?.Debug("=== Packet Info ===");
            Log?.Debug("Name: {0}", Configuration.Identity.Name);
            Log?.Debug("Channel: {0}", Configuration.Identity.Channel);
            Log?.Debug("Role: {0}", Role);
            Log?.Debug("===================");
            Log?.Debug("");

            // Send a InternalHello packet
            Send(
                ToffeeOpCode.ClientHello,
                new InternalHello
                {
                    Name = Configuration.Identity.Name,
                    Channel = Configuration.Identity.Channel,
                    Role = Role
                }
            );
        }

        /// <summary>
        /// Handles the decryption, decompression and basic decoding of received client packets.
        /// </summary>
        /// <param name="packet">The received packet.</param>
        sealed protected override void HandlePacket(byte[] packet)
        {

        }

        /// <summary>
        /// Handles a decrypted/decompressed/decoded packet.
        /// </summary>
        /// <param name="opCode">The OpCode of the received packet.</param>
        /// <param name="packet">The received packet data.</param>
        protected virtual void HandlePacket(uint opCode, byte[] packet)
        {
            switch ((ToffeeOpCode)opCode)
            {
                default:
                    break;
            }
        }

        /// <summary>
        /// Sends a packet to the server.
        /// </summary>
        /// <param name="packet">The data to send.</param>
        public void Send(ToffeeInternalPacket packet)
        {
            Send(packet.BuildPacket());
        }

        /// <summary>
        /// Builds a packet, and sends it to the server.
        /// </summary>
        /// <param name="opCode">The OpCode of the packet to build.</param>
        /// <param name="o">The packet data.</param>
        public void Send<T>(ushort opCode, T o)
        {
            Send(ToffeeInternalPacket.Create(this, opCode, o).BuildPacket());
        }

        /// <summary>
        /// Builds a packet, and sends it to the server.
        /// </summary>
        /// <param name="opCode">The OpCode of the packet to build.</param>
        /// <param name="o">The packet data.</param>
        public void Send<T>(ToffeeOpCode opCode, T o) where T : new()
        {
            Send((ushort)opCode, o);
        }
    }
}

