using System.Net.Sockets;

using Toffee.Core;
using Toffee.Protocol;
using Toffee.Server.Internal;
using Toffee.Logging;


namespace Toffee.Server.Roles.MessageDirector
{
    public class MessageDirectorServer : ToffeeServer
    {
        public MessageDirectorConfiguration Configuration { get; private set; }

        public MessageDirectorServer(MessageDirectorConfiguration configuration, LoggerCategory category)
            : base(configuration, category)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Handles recieved packets.
        /// </summary>
        /// <param name="session">The session that received the packet.</param>
        /// <param name="packet">The packet data that was received.</param>
        public override void HandlePacket(ToffeeSession session, byte[] packet)
        {
            // Get the session
            ToffeeSession sender = session;

            // Read the packet
            ToffeeInternalPacketReadResult result = ToffeeInternalPacket.Read(session, packet);

            // Was this a succcessful read?
            if (result.Success)
            {
                Log?.Debug("Received 0x{0:X2} from session {1}.", result.Header.OpCode, sender.SessionId);

                ToffeePacketIterator iterator = new ToffeePacketIterator(session, result.Data);
                switch (result.Header.OpCode)
                {
                    case (ushort)ToffeeOpCode.InternalHello:
                        break;
                }
            }
            else
                Log?.Warning("Received corrupt packet from session {1}.", sender.SessionId);
        }
    }
}
