using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Toffee.Core.Parser.Definitions;
using Toffee.Util;
using Toffee.Logging;

namespace Toffee.Server.Roles.ClientAgent
{
    [ToffeeRole("ClientAgent")]
    public class ClientAgentRole : IRole
    {
        public static int ClientAgentCount { get; private set; }

        public ClientAgentServer Server { get; private set; }
        public int UniqueClientAgentIndex { get; private set; }

        public ClientAgentRole()
        {
            UniqueClientAgentIndex = ClientAgentCount++;
        }

        public bool Configure(object config, Logger logger = null)
        {
            try
            {
                // Get the configuration
                JObject jConfig = (JObject)config;
                ClientAgentConfiguration configuration = jConfig.ToObject<ClientAgentConfiguration>();

                // Load the network files
                TdlFile networkFile = new TdlFile(configuration.Network.Namespace, false);
                foreach (string file in configuration.Network.Files)
                {
                    if (!networkFile.ParseFile(file))
                    {
                        Console.WriteLine("Could not find: {0}", file);
                        return false;
                    }
                }

                // Create the logger category
                LoggerCategory category = 
                    logger?.MakeCategory(string.Format("ClientAgent-{0}", UniqueClientAgentIndex)) ?? null;

                // Sanity checks
                if (configuration.Internal.ChannelCount != 0)
                {
                    category.Warning("Internal channel count should not be set manually. Resetting value to maximum connections.");
                    configuration.Internal.ChannelCount = configuration.MaximumConnections;
                }

                // Create the server
                Server = new ClientAgentServer(configuration, networkFile, category);
                return true;
            }
            catch (JsonException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        
        public void Tick()
        {
            // Let's go through each session
            foreach (ClientAgentSession session in Server.SessionManager.Sessions)
            {
                long timestamp = TimeUtil.GetUnixTimestamp();
                // Have they taken too long to validate?
                if ((!session.Validated) && (timestamp - session.ConnectTime > 2))
                    session.Disconnect();

                // Have they taken too long to heartbeat?
                if ((timestamp - session.LastHeartbeat) > Server.Configuration.HeartbeatTime + 2)
                    session.Disconnect();
            }
        }

        public void Start()
        {
            Server.Start();
        }

        public void Stop()
        {
            // Disconnect each session
            foreach (ClientAgentSession session in Server.SessionManager.Sessions)
                session.Disconnect();
            Server.Stop();
        }
    }
}
