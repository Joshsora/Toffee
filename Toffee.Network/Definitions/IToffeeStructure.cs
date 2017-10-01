namespace Toffee.Core.Definitions
{
    public interface IToffeeStructure
    {
        void WriteTo(ToffeePacket packet);
        void ReadFrom(ToffeePacketIterator packetIterator);
    }
}
