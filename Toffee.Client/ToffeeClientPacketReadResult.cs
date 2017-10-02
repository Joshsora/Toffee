using Toffee.Protocol.Packets.Client;

namespace Toffee.Client
{
    public struct ToffeeClientPacketReadResult
    {
        public bool Success { get; private set; }
        public bool Encrypted { get; private set; }
        public ClientPacketHeader Header { get; private set; }
        public byte[] Data { get; private set; }

        public ToffeeClientPacketReadResult(bool success, bool encrypted, ClientPacketHeader header, byte[] data)
        {
            Success = success;
            Encrypted = encrypted;
            Header = header;
            Data = data;
        }
    }
}
