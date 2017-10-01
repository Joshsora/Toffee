using System.Timers;
using Toffee.Core;
using Toffee.Core.Packets.Client;
using System;

namespace Toffee.Client
{
    public partial class ToffeeClient
    {
        protected virtual void HandleClientHelloResponse(byte[] packet)
        {
            // Get the response
            ToffeePacketIterator iterator = new ToffeePacketIterator(this, packet);
            ClientHelloResponse response = iterator.ReadStruct<ClientHelloResponse>();

            // Was the request successful?
            if (response.Success)
            {
                // Does this server require a heartbeat?
                if (response.HeartbeatTime != 0)
                {
                    HeartbeatTimer = new Timer(response.HeartbeatTime * 1000);
                    HeartbeatTimer.Elapsed += SendHeartbeat;
                    HeartbeatTimer.Start();
                }

                // Do we have encryption keys?
                ForceEncryption = response.ForcedEncryption;
                if ((!UseEncryption) && (response.ForcedEncryption))
                {
                    // The server requires encryption that we don't have enabled.
                    Disconnect();
                    return;
                }
            }
            else
            {
                // OnConnectionFailed(this);
                Disconnect();
            }
        }

        private void HandleClientHeartbeat()
        {
            PingWatch.Stop();
            Ping = PingWatch.ElapsedMilliseconds;
            Console.WriteLine("{0}ms ping", Ping);
            PingWatch.Reset();
        }
    }
}
