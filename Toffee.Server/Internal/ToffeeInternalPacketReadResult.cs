using Toffee.Protocol.Packets.Internal;

namespace Toffee.Server.Internal
{
    public struct ToffeeInternalPacketReadResult
    {
        public bool Success { get; private set; }
        public InternalPacketHeader Header { get; private set; }
        public byte[] Data { get; private set; }

        public ToffeeInternalPacketReadResult(bool success, InternalPacketHeader header, byte[] data)
        {
            Success = success;
            Header = header;
            Data = data;
        }
    }
}
