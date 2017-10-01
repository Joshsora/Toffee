using Toffee.Core.Definitions;

namespace Toffee.Client
{
    public class ToffeeClientSettings
    {
        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string NetworkName { get; set; }
        public ulong EncryptionKey { get; set; }

        public ToffeeClientSettings()
        {
            AppName = "Game";
            AppVersion = "1.0.0.0";
            NetworkName = ToffeeNetwork.StandardToffeeNetwork;
            EncryptionKey = 0x00;
        }
    }
}
