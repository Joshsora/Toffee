using System.Net.Sockets;
using System.Timers;
using System.Diagnostics;

using Toffee.Core;
using Toffee.Protocol;
using Toffee.Protocol.Packets.Client;

namespace Toffee.Client
{
    /// <summary>
    /// A ToffeeParticipant that implements the client protocol.
    /// </summary>
    public partial class ToffeeClient : ToffeeParticipant
    {
        /// <summary>
        /// True if we have authenticated with the server successfully, and false otherwise.
        /// </summary>
        public bool Authenticated { get; private set; }

        /// <summary>
        /// A Timer that controls heartbeat intervals.
        /// </summary>
        private Timer HeartbeatTimer { get; set; }

        /// <summary>
        /// The connection's ping.
        /// </summary>
        public long Ping { get; private set; }

        /// <summary>
        /// A Stopwatch to time ping.
        /// </summary>
        private Stopwatch PingWatch { get; set; }

        /// <summary>
        /// The settings that are currently being used by this ToffeeClient.
        /// </summary>
        public ToffeeClientSettings ClientSettings { get; private set; }

        /// <summary>
        /// Creates a ToffeeClient instance with the default settings.
        /// </summary>
        public ToffeeClient() : this(new ToffeeClientSettings()) { }

        /// <summary>
        /// Creates a ToffeeClient instance with the settings provided.
        /// </summary>
        /// <param name="settings">The settings to use.</param>
        public ToffeeClient(ToffeeClientSettings settings)
            : base(settings.NetworkName, settings.EncryptionKey)
        {
            ClientSettings = settings;
        }

        /// <summary>
        /// Called when a connection attempt is successful.
        /// </summary>
        protected override void ConnectionEstablished()
        {
            PingWatch = new Stopwatch();

            // Send a ClientHello packet
            Send(
                ToffeeOpCode.ClientHello,
                new ClientHello
                {
                    // Game information
                    AppName = ClientSettings.AppName,
                    AppVersion = ClientSettings.AppVersion,

                    // Network information
                    NetworkHash = Network.Hash,

                    // Encryption info
                    HasEncryption = UseEncryption
                }
            );
        }

        /// <summary>
        /// Handles the decryption, decompression and basic decoding of received client packets.
        /// </summary>
        /// <param name="packet">The received packet.</param>
        sealed protected override void HandlePacket(byte[] packet)
        {
            // Read the packet
            ToffeeClientPacketReadResult result = ToffeeClientPacket.Read(this, packet);
            if (!result.Success)
            {
                // This packet was malformed..
                // TODO
                return;
            }
            
            // If we have encryption, set the last timestamp we received
            if (UseEncryption)
                Encryption.LastServerTimestamp = result.Header.Timestamp;

            // Handle the packet!
            HandlePacket(result.Header.OpCode, result.Data);
        }

        /// <summary>
        /// Handles a decrypted/decompressed/decoded packet.
        /// </summary>
        /// <param name="opCode">The OpCode of the received packet.</param>
        /// <param name="packet">The received packet data.</param>
        protected void HandlePacket(uint opCode, byte[] packet)
        {
            switch ((ToffeeOpCode)opCode)
            {
                case ToffeeOpCode.ClientHelloResponse:
                    HandleClientHelloResponse(packet);
                    break;

                case ToffeeOpCode.ClientHeartbeat:
                    HandleClientHeartbeat();
                    break;

                default:
                    HandleCustomPacket(opCode, packet);
                    break;
            }
        }

        /// <summary>
        /// Handles an application-specific packet.
        /// </summary>
        /// <param name="opCode">The OpCode of the received packet.</param>
        /// <param name="packet">The received packet data.</param>
        protected virtual void HandleCustomPacket(uint opCode, byte[] packet)
        {

        }

        /// <summary>
        /// Sends a heartbeat to the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendHeartbeat(object sender, ElapsedEventArgs e)
        {
            ToffeeClientPacket tcp = new ToffeeClientPacket(this, (ushort)ToffeeOpCode.ClientHeartbeat);
            Send(tcp.BuildPacket());
            PingWatch.Start();
        }

        /// <summary>
        /// Sends a packet to the server.
        /// </summary>
        /// <param name="packet">The data to send.</param>
        public void Send(ToffeeClientPacket packet)
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
            Send(ToffeeClientPacket.Create(this, opCode, o).BuildPacket());
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
