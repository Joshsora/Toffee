using System;
using System.Net.Sockets;
using Toffee.Protocol.Objects;
using Toffee.Protocol.Services;
using Toffee.Protocol.Definitions;
using Toffee.Util;

namespace Toffee.Protocol
{
    /// <summary>
    /// A very simple network participant to a Toffee network.
    /// </summary>
    public abstract class ToffeeParticipant
    {
        /// <summary>
        /// The lower-level Socket.
        /// </summary>
        protected Socket Socket { get; set; }

        /// <summary>
        /// The TCP Stream.
        /// </summary>
        protected NetworkStream Stream { get; private set; }

        /// <summary>
        /// The IP Address or Hostname that we are currently connected to.
        /// </summary>
        public string Host { get; protected set; }

        /// <summary>
        /// True if encryption is configured, and false otherwise.
        /// </summary>
        public bool UseEncryption
        {
            get
            {
                return Encryption != null;
            }
        }

        /// <summary>
        /// True if the client/server expects all packets to be encrypted, and false otherwise.
        /// </summary>
        public bool ForceEncryption { get; set; }

        /// <summary>
        /// The EncryptionService that has been configured for this connection.
        /// </summary>
        public EncryptionService Encryption { get; protected set; }

        /// <summary>
        /// The ToffeeNetwork for this connection.
        /// </summary>
        public ToffeeNetwork Network { get; protected set; }

        /// <summary>
        /// The ObjectContainer that contains all objects that this connection has been made aware of.
        /// </summary>
        public ObjectContainer Objects { get; protected set; }

        /// <summary>
        /// The ServiceContainer that contains all services that this connection has been made aware of.
        /// </summary>
        public ServiceContainer Services { get; protected set; }

        /// <summary>
        /// The last time a successful connection occurred.
        /// </summary>
        public long ConnectTime { get; protected set; }

        /// <summary>
        /// Called when a connection could not be established.
        /// </summary>
        public event Action ConnectionFailed;

        /// <summary>
        /// Called when the connection was lost.
        /// </summary>
        public event Action ConnectionLost;

        /// <summary>
        /// A lock used to prevent multiple BeginWrite operations.
        /// </summary>
        private object SendLock;

        /// <summary>
        /// Creates a ToffeeParticipant instance with the settings provided.
        /// </summary>
        /// <param name="networkName"></param>
        /// <param name="encryptionKey"></param>
        public ToffeeParticipant(string networkName, ulong encryptionKey)
        {
            // Set the empty events
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectionFailed = delegate { };
            ConnectionLost = delegate { };

            // Protocol settings
            if (encryptionKey != 0)
                Encryption = new EncryptionService(encryptionKey);

            // Empty containers
            Network = ToffeeNetwork.CreateNetwork(networkName);
            Objects = new ObjectContainer();
            Services = new ServiceContainer();
            SendLock = new object();
        }

        /// <summary>
        /// Creates a ToffeeParticipant instance with the settings provided.
        /// </summary>
        /// <param name="socket">The connection's Socket.</param>
        /// <param name="networkName">The name of the network.</param>
        /// <param name="encryptionKey">The encryption key to base encryption on.</param>
        public ToffeeParticipant(Socket socket, string networkName, ulong encryptionKey)
            : this(networkName, encryptionKey)
        {
            if (socket == null)
                throw new ArgumentNullException("socket");
            Socket = socket;
            Stream = new NetworkStream(socket);
        }

        /// <summary>
        /// Attempt to start a connection with the specified host and port.
        /// Calls 'ConnectionEstablished' if successful, and 'ConnectionFailed' otherwise.
        /// </summary>
        /// <param name="host">The IP address or hostname to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        public void Connect(string host, int port)
        {
            if (Socket == null)
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Are we already connected to something?
            if (Socket.Connected)
                return;

            // Set the host, and start connecting
            Host = host;
            try
            {
                // Start trying to establish a connection
                Socket.BeginConnect(host, port, OnConnectionEstablished, null);
            }
            catch
            {
                // Something went wrong while trying to connect
                ConnectionFailed();
            }
        }

        /// <summary>
        /// Called when a connection attempt is successful.
        /// </summary>
        /// <param name="ar">The result of the async operation.</param>
        private void OnConnectionEstablished(IAsyncResult ar)
        {
            try
            {
                // Try and end the async connect operation
                Socket.EndConnect(ar);
            }
            catch
            {
                // Something went wrong while trying to connect
                ConnectionFailed();
                return;
            }

            // Setup a NetworkStream
            Stream = new NetworkStream(Socket);

            // Set the connect time
            ConnectTime = TimeUtil.GetUnixTimestamp();

            // Start receiving data
            BeginReceivePacket();

            // Call the event
            ConnectionEstablished();
        }

        /// <summary>
        /// Called when a connection has successfully been established.
        /// </summary>
        protected virtual void ConnectionEstablished()
        {
            // Filled in by subclasses
        }

        /// <summary>
        /// Begins receiving a packet.
        /// </summary>
        protected void BeginReceivePacket()
        {
            try
            {
                // Start receiving the packet size
                ToffeeReceiveState state = new ToffeeReceiveState(2);
                Stream.BeginRead(state.Buffer, 0, 2, ReceivedPacketSize, state);
            }
            catch
            {
                // Something went wrong while trying to receive data
                Disconnect();
            }
        }

        /// <summary>
        /// Callback after 'BeginReceivePacket'.
        /// Called when there is data available for the packet size.
        /// </summary>
        /// <param name="ar">The result of the async operation.</param>
        private void ReceivedPacketSize(IAsyncResult ar)
        {
            try
            {
                // End the receive, and get the state
                int length = Socket.EndReceive(ar);
                ToffeeReceiveState state = (ToffeeReceiveState)ar.AsyncState;
                state.ReceivedLength += length;

                // Is there still more data we're expecting?
                if (!state.Received)
                {
                    // Keep receiving data for the size
                    Stream.BeginRead(
                        state.Buffer, state.ReceivedLength, state.AwaitingLength, ReceivedPacketSize, state);
                    return;
                }

                // We've got the entire size!
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(state.Buffer);
                ushort packetSize = BitConverter.ToUInt16(state.Buffer, 0);
                state = new ToffeeReceiveState(packetSize);
                Stream.BeginRead(
                    state.Buffer, 0, packetSize, ReceivedPacket, state);
            }
            catch
            {
                // Something went wrong while trying to receive data
                Disconnect();
            }
        }

        /// <summary>
        /// Callback after the packet size has been received.
        /// Called when there is data available for the packet.
        /// </summary>
        /// <param name="ar">The result of the async operation.</param>
        private void ReceivedPacket(IAsyncResult ar)
        {
            byte[] packet = null;
            try
            {
                // End the receive, and get the state
                int length = Socket.EndReceive(ar);
                ToffeeReceiveState state = (ToffeeReceiveState)ar.AsyncState;
                state.ReceivedLength += length;

                // Is there still more data we're expecting?
                if (!state.Received)
                {
                    // Keep receiving data for the size
                    Stream.BeginRead(
                        state.Buffer, state.ReceivedLength, state.AwaitingLength, ReceivedPacket, state);
                    return;
                }

                // Begin receiving again
                packet = state.Buffer;
                BeginReceivePacket();
            }
            catch
            {
                // Something went wrong while trying to receive data
                Disconnect();
            }

            // Handle the packet
            if (packet != null)
                HandlePacket(packet);
        }

        /// <summary>
        /// Handles recieved packets.
        /// </summary>
        /// <param name="packet">The packet data that was received.</param>
        protected virtual void HandlePacket(byte[] packet)
        {
            // Filled in by subclasses
        }

        /// <summary>
        /// Sends a packet to the server.
        /// </summary>
        /// <param name="packet">The data to send.</param>
        public void Send(byte[] packet)
        {
            // Are we even connected to anything?
            if (!Socket.Connected)
                return;

            // Prepend the length of the packet
            ToffeePacket fullPacket = new ToffeePacket(this);
            fullPacket.Write((ushort)packet.Length);
            fullPacket.Write(packet, true);

            // Send!
            byte[] fullBytes = fullPacket.GetBytes();
            lock (SendLock)
            {
                Stream.BeginWrite(fullBytes, 0, fullBytes.Length, SendCallback, null);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Stream.EndWrite(ar);
            }
            catch
            {
                // Something went wrong while trying to receive data
                Disconnect();
            }
        }

        protected void OnConnectionLost()
        {
            ConnectionLost();
        }

        /// <summary>
        /// Disconnects this participant from the client/server.
        /// </summary>
        public virtual void Disconnect()
        {
            // Are we even connected to anything?
            if (!Socket.Connected)
                return;
            Socket.Dispose();
            Stream.Dispose();
            OnConnectionLost();
        }
    }
}
