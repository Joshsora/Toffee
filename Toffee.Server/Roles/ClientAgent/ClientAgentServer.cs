using System.Net.Sockets;
using Toffee.Core;
using Toffee.Protocol;
using Toffee.Protocol.Packets.Client;
using Toffee.Core.Parser.Definitions;
using Toffee.Client;
using Toffee.Util;
using Toffee.Logging;
using Toffee.Server.Internal;

namespace Toffee.Server.Roles.ClientAgent
{
    public class ClientAgentServer : ToffeeServer
    {
        public ClientAgentConfiguration Configuration { get; private set; }
        public TdlFile NetworkFile { get; private set; }
        public ToffeeInternalClient InternalClient { get; private set; }

        public ClientAgentServer(ClientAgentConfiguration configuration, TdlFile file, LoggerCategory category)
            : base(configuration, category)
        {
            Configuration = configuration;
            NetworkFile = file;

            // Create the internal connection
            InternalClient = new ToffeeInternalClient(configuration.Internal, "ClientAgent", category);
        }

        /// <summary>
        /// Handles new connections.
        /// </summary>
        /// <param name="socket">The connection socket.</param>
        protected override void HandleConnection(Socket socket)
        {
            ClientAgentSession session = new ClientAgentSession(SessionManager, socket, 0);
            SessionManager.NewSession(session);
            Log?.Info("Began new session. (id: {0}, channel: {1})", session.SessionId, session.ClientChannel);
        }

        /// <summary>
        /// Handles recieved packets.
        /// </summary>
        /// <param name="session">The session that received the packet.</param>
        /// <param name="packet">The packet data that was received.</param>
        public override void HandlePacket(ToffeeSession session, byte[] packet)
        {
            // Get the session
            ClientAgentSession sender = (ClientAgentSession)session;

            // Read the packet
            ToffeeClientPacketReadResult result = ToffeeClientPacket.Read(session, packet);

            // Was this a succcessful read?
            if (result.Success)
            {
                Log?.Debug("Received 0x{0:X2} from session {1}.", result.Header.OpCode, sender.SessionId);

                ToffeePacketIterator iterator = new ToffeePacketIterator(session, result.Data);
                switch (result.Header.OpCode)
                {
                    case (ushort)ToffeeOpCode.ClientHello:
                        HandleClientHello(sender, iterator.ReadStruct<ClientHello>());
                        break;

                    case (ushort)ToffeeOpCode.ClientHeartbeat:
                        HandleClientHeartbeat(sender);
                        break;
                }
            }
            else
                Log?.Warning("Received corrupt packet from session {1}.", sender.SessionId);
        }

        /// <summary>
        /// Handles a ClientHello packet.
        /// </summary>
        /// <param name="session">The session that sent this packet.</param>
        /// <param name="hello">The ClientHello data that was sent.</param>
        private void HandleClientHello(ClientAgentSession sender, ClientHello hello)
        {
            // Logging
            Log?.Info("Handling ClientHello from session {0}.", sender.SessionId);

            // Let's see if this client is valid...
            ClientHelloResponse response = new ClientHelloResponse
            {
                Success = true,
                ErrorCode = 0,
                Message = "",
                HeartbeatTime = Configuration.HeartbeatTime,
                ForcedEncryption = Configuration.ForceEncryption
            };

            // Get the correct wanted hash
            uint wantedHash = Configuration.Network.Hash != 0 ?
                Configuration.Network.Hash : NetworkFile.Hash;

            // Is the client the wrong application?
            if (hello.AppName != Configuration.Application.Name)
            {
                response.Success = false;
                response.ErrorCode = (byte)ToffeeErrorCodes.ClientHelloApplicationInvalid;
                Log?.Warning("Invalid ClientHello from session {0}. ({1})",
                    sender.SessionId, ToffeeErrorCodes.ClientHelloApplicationInvalid);
            }
            // Is the client outdated?
            else if (hello.AppVersion != Configuration.Application.Version)
            {
                response.Success = false;
                response.ErrorCode = (byte)ToffeeErrorCodes.ClientHelloApplicationOutOfDate;
                Log?.Warning("Invalid ClientHello from session {0}. ({1})",
                    sender.SessionId, ToffeeErrorCodes.ClientHelloApplicationOutOfDate);
            }
            // Is their encryption the same as ours?
            else if ((Configuration.EncryptionKey == 0x00) && (hello.HasEncryption))
            {
                response.Success = false;
                response.ErrorCode = (byte)ToffeeErrorCodes.ClientHelloEncryptionInvalid;
                response.Message = "The server is not configured to have encryption.";
                Log?.Warning("Invalid ClientHello from session {0}. ({1}, {2})",
                    sender.SessionId, ToffeeErrorCodes.ClientHelloEncryptionInvalid, response.Message);
            }
            // If we force encryption, do they have encryption?
            else if ((Configuration.ForceEncryption) && (!hello.HasEncryption))
            {
                response.Success = false;
                response.ErrorCode = (byte)ToffeeErrorCodes.ClientHelloEncryptionInvalid;
                response.Message = "The server is configured to force encryption.";
                Log?.Warning("Invalid ClientHello from session {0}. ({1}, {2})",
                    sender.SessionId, ToffeeErrorCodes.ClientHelloEncryptionInvalid, response.Message);
            }
            // Do they have the same network hash?
            else if (hello.NetworkHash != wantedHash)
            {
                response.Success = false;
                response.ErrorCode = (byte)ToffeeErrorCodes.ClientHelloHashInvalid;
                Log?.Warning("Invalid ClientHello from session {0}. ({1})",
                    sender.SessionId, ToffeeErrorCodes.ClientHelloHashInvalid);
            }
            else
            {
                sender.Validated = true;
                Log?.Info("ClientHello was valid! Session {0} has been validated.", sender.SessionId);
            }

            // Send the response!
            sender.Send(ToffeeOpCode.ClientHelloResponse, response);

            // Debug information
            Log?.Debug("=== Response Info ===");
            Log?.Debug("OpCode: 0x02 (ClientHelloResponse)");
            Log?.Debug("Success: {0}", response.Success);
            Log?.Debug("ErrorCode: {0}", response.ErrorCode);
            Log?.Debug("Message: {0}", response.Message);
            Log?.Debug("HeartbeatTime: {0}", response.HeartbeatTime);
            Log?.Debug("ForcedEncryption: {0}", response.ForcedEncryption);
            Log?.Debug("=====================");
            Log?.Debug("");
        }

        /// <summary>
        /// Handles a client heartbeat.
        /// </summary>
        /// <param name="session">The session that sent this packet.</param>
        private void HandleClientHeartbeat(ClientAgentSession sender)
        {
            Log?.Info("Handling ClientHeartbeat from session {0}.", sender.SessionId);
            sender.LastHeartbeat = TimeUtil.GetUnixTimestamp();

            // Send a pong
            sender.Send(new ToffeeClientPacket(sender, ToffeeOpCode.ClientHeartbeat));

            // Debug information
            Log?.Debug("=== Response Info ===");
            Log?.Debug("OpCode: 0x04 (ClientHeartbeat)");
            Log?.Debug("=====================");
            Log?.Debug("");
        }
    }
}
