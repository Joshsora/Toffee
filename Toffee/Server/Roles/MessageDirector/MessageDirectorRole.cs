using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Toffee.Logging;

namespace Toffee.Server.Roles.MessageDirector
{
    [ToffeeRole("MessageDirector")]
    public class MessageDirectorRole : IRole
    {
        public static int MessageDirectorCount { get; private set; }

        public MessageDirectorServer Server { get; private set; }
        public int UniqueMessageDirectorIndex { get; private set; }

        public MessageDirectorRole()
        {
            UniqueMessageDirectorIndex = MessageDirectorCount++;
        }

        public bool Configure(object config, Logger logger = null)
        {
            try
            {
                // Get the configuration
                JObject jConfig = (JObject)config;
                MessageDirectorConfiguration configuration = jConfig.ToObject<MessageDirectorConfiguration>();

                // Create the server
                LoggerCategory category = null;
                if (logger != null)
                    category = logger.MakeCategory(string.Format("MessageDirector-{0}", UniqueMessageDirectorIndex));
                Server = new MessageDirectorServer(configuration, category);
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

        }

        public void Start()
        {
            Server.Start();
        }

        public void Stop()
        {
            Server.Stop();
        }
    }
}
