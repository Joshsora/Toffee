using System.Net.Sockets;
using Toffee.Core;
using Toffee.Util;

namespace Toffee.Server
{
    /// <summary>
    /// A class that represents a ToffeeParticipant on the server-side.
    /// </summary>
    public class ToffeeSession : ToffeeParticipant
    {
        /// <summary>
        /// The manager component mangaging this session.
        /// </summary>
        public ToffeeSessionManager Manager { get; private set; }

        /// <summary>
        /// This session's unique identifier on this server.
        /// NOTE: This is not globally unique between servers.
        /// </summary>
        public int SessionId { get; internal set; }

        /// <summary>
        /// True if the session has sent a valid ClientHello.
        /// </summary>
        public bool Validated { get; internal set; }

        public ToffeeSession(ToffeeSessionManager manager, Socket socket)
            : base(socket, manager.Server.Network.Name, manager.Server.EncryptionKey)
        {
            Manager = manager;
            Validated = false;
            ConnectTime = TimeUtil.GetUnixTimestamp();

            // Begin receiving packets
            BeginReceivePacket();
        }

        /// <summary>
        /// Handles recieved packets.
        /// </summary>
        /// <param name="packet">The packet data that was received.</param>
        protected override void HandlePacket(byte[] packet)
        {
            Manager.Server.HandlePacket(this, packet);
        }

        /// <summary>
        /// Disconnects this session from the server.
        /// </summary>
        public override void Disconnect()
        {
            // TODO: Reason
            // Are we already disconnected?
            if (!Socket.Connected)
            {
                // Alert the manager again just in-case it's kept around
                Manager.SessionDisconnected(this);
                return;
            }

            // Disconnect the session, and alert the manager
            Socket.Close();
            OnConnectionLost();
            Manager.SessionDisconnected(this);
        }
    }
}
