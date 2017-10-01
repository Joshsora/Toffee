using System;
using System.Net;
using System.Net.Sockets;

using Toffee.Core.Definitions;
using Toffee.Logging;

namespace Toffee.Server
{
    public class ToffeeServer
    {
        /// <summary>
        /// The underlying TcpListner.
        /// </summary>
        private TcpListener Listener { get; set; }

        /// <summary>
        /// True if the server is currently accepting connections.
        /// </summary>
        public bool Listening { get; private set; }

        /// <summary>
        /// The port the server is listening on.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// The Network the server is using.
        /// </summary>
        public ToffeeNetwork Network { get; private set; }
        
        /// <summary>
        /// The manager in charge of this server's sessions.
        /// </summary>
        public ToffeeSessionManager SessionManager { get; private set; }

        /// <summary>
        /// The maximum number of connections this server is able to have connected at once.
        /// If this value is zero, then there is no limits.
        /// </summary>
        public uint MaximumConnections { get; private set; }

        /// <summary>
        /// True if the client/server expects all packets to be encrypted, and false otherwise.
        /// </summary>
        public bool ForceEncryption { get; private set; }

        /// <summary>
        /// The encryption key.
        /// </summary>
        public ulong EncryptionKey { get; private set; }

        /// <summary>
        /// This server's logging category.
        /// </summary>
        public LoggerCategory Log { get; private set; }

        /// <summary>
        /// Creates a new ToffeeServer.
        /// </summary>
        /// <param name="configuration">The configuration to use for this server.</param>
        public ToffeeServer(ToffeeServerConfiguration configuration, LoggerCategory category = null)
        {
            // Create the TCP listener
            Listener = new TcpListener(new IPEndPoint(IPAddress.Any, configuration.Port));
            Listening = false;

            // Set the properties of this server depending on the values in the configuration
            Port = configuration.Port;
            MaximumConnections = configuration.MaximumConnections;
            ForceEncryption = configuration.ForceEncryption;
            EncryptionKey = configuration.EncryptionKey;

            // Sanity check for encryption
            if ((ForceEncryption) && (EncryptionKey == 0x00))
            {
                Log?.Warning("Cannot force encryption, and have no encryption key! Setting 'ForceEncryption' to false.");
                ForceEncryption = false;
            }

            // Create the components
            if (configuration.Network != null)
                Network = ToffeeNetwork.CreateNetwork(configuration.Network.Name, configuration.Network.Suffix);
            else
                Network = ToffeeNetwork.CreateNetwork(ToffeeNetwork.StandardToffeeNetwork);
            SessionManager = new ToffeeSessionManager(this);
            Log = category;
            Log?.Info("Created.");
        }

        public ToffeeServer(int port, LoggerCategory category = null)
        {
            Listener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
            Listening = false;
            Port = port;

            Network = ToffeeNetwork.CreateNetwork(ToffeeNetwork.StandardToffeeNetwork);
            SessionManager = new ToffeeSessionManager(this);
            Log = category;
            Log?.Info("Created.");
        }

        /// <summary>
        /// Start accepting connections.
        /// </summary>
        public void Start()
        {
            if (!Listening)
            {
                Listener.Start();
                Listening = true;
                Log?.Info("Started listening on port: {0}.", Port);
            }

            try
            {
                if (Listening)
                    Listener.BeginAcceptSocket(OnConnectionAccepted, null);
            }
            catch
            {
                Log?.Error("Stopped listening for connections. (Error occurred!)");
                Stop();
            }
        }

        /// <summary>
        /// Stop accepting connections.
        /// </summary>
        public void Stop()
        {
            Listening = false;
            Listener.Stop();
        }

        public void SessionDisconnected()
        {
            // Check if we can accept more connections now
            if (!Listening)
                if (SessionManager.Sessions.Length <= MaximumConnections)
                    Start();
        }

        /// <summary>
        /// Called when a connection has been established.
        /// </summary>
        /// <param name="ar">The result of the async operation.</param>
        private void OnConnectionAccepted(IAsyncResult ar)
        {
            try
            {
                Socket sessionSocket = Listener.EndAcceptSocket(ar);
                HandleConnection(sessionSocket);

                // Have we accepted the maximum connections?
                if ((MaximumConnections != 0) && (SessionManager.Sessions.Length >= MaximumConnections))
                {
                    Log?.Info("Stopped listening for connections. (Reached maximum connections)");
                    Stop();
                }

                // Are we still listening?
                if (Listening)
                    Start();
            }
            catch
            {
                Log?.Error("Stopped listening for connections. (Error occurred!)");
                Stop();
            }
        }

        /// <summary>
        /// Handles new connections.
        /// </summary>
        /// <param name="socket">The connection socket.</param>
        protected virtual void HandleConnection(Socket socket)
        {
            ToffeeSession session = new ToffeeSession(SessionManager, socket);
            SessionManager.NewSession(session);
            Log?.Info("Began new session. (id: {0})", session.SessionId);
        }

        /// <summary>
        /// Handles recieved packets.
        /// </summary>
        /// <param name="session">The session that received the packet.</param>
        /// <param name="packet">The packet data that was received.</param>
        public virtual void HandlePacket(ToffeeSession session, byte[] packet)
        {
            Log?.Warning("Handle packet. (This should be overridden!)");
        }
    }
}
