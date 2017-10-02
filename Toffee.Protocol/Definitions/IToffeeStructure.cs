namespace Toffee.Protocol.Definitions
{
    public interface IToffeeStructure
    {
        void WriteTo(ToffeePacket packet);
        void ReadFrom(ToffeePacketIterator packetIterator);
    }
}
