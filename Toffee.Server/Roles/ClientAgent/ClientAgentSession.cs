using System.Net.Sockets;
using Toffee.Core;
using Toffee.Client;
using Toffee.Util;

namespace Toffee.Server.Roles.ClientAgent
{
    public class ClientAgentSession : ToffeeSession
    {
        /// <summary>
        /// The server that this session belongs to.
        /// </summary>
        public ClientAgentServer Server
        {
            get
            {
                return (ClientAgentServer)Manager.Server;
            }
        }

        /// <summary>
        /// The channel that this session has been allocated.
        /// </summary>
        public uint ClientChannel { get; private set; }

        /// <summary>
        /// The last time a heartbeat was received.
        /// </summary>
        public long LastHeartbeat { get; set; }

        public ClientAgentSession(ToffeeSessionManager manager, Socket socket, uint channel)
            : base(manager, socket)
        {
            ClientChannel = channel;
            LastHeartbeat = TimeUtil.GetUnixTimestamp();
        }

        /// <summary>
        /// Sends a packet to the session.
        /// </summary>
        /// <param name="packet">The data to send.</param>
        public void Send(ToffeeClientPacket packet)
        {
            Send(packet.BuildPacket());

            // If we use encryption, make sure to set the last server timestamp
            if (UseEncryption)
                Encryption.LastServerTimestamp = packet.LastHeader.Timestamp;
        }

        /// <summary>
        /// Builds a packet, and sends it to the session.
        /// </summary>
        /// <param name="opCode">The OpCode of the packet to build.</param>
        /// <param name="o">The packet data.</param>
        public void Send<T>(ushort opCode, T o)
        {
            ToffeeClientPacket packet = ToffeeClientPacket.Create(this, opCode, o);
            Send(packet.BuildPacket());

            // If we use encryption, make sure to set the last server timestamp
            if (UseEncryption)
                Encryption.LastServerTimestamp = packet.LastHeader.Timestamp;
        }

        /// <summary>
        /// Builds a packet, and sends it to the session.
        /// </summary>
        /// <param name="opCode">The OpCode of the packet to build.</param>
        /// <param name="o">The packet data.</param>
        public void Send<T>(ToffeeOpCode opCode, T o)
        {
            Send((ushort)opCode, o);
        }
    }
}
