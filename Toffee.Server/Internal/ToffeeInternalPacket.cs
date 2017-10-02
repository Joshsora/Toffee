using Toffee.Core;
using Toffee.Protocol;
using Toffee.Protocol.Packets.Internal;
using Toffee.Util;

namespace Toffee.Server.Internal
{
    public class ToffeeInternalPacket : ToffeePacket
    {
        /// <summary>
        /// The OpCode of the packet.
        /// </summary>
        public ushort OpCode { get; private set; }

        /// <summary>
        /// The Header that was created for this packet last time 'BuildPacket' was called.
        /// </summary>
        public InternalPacketHeader LastHeader { get; private set; }

        public ToffeeInternalPacket(ToffeeParticipant client, ushort opCode) : base(client)
        {
            OpCode = opCode;
        }

        public ToffeeInternalPacket(ToffeeParticipant client, ToffeeOpCode opCode) : base(client)
        {
            OpCode = (ushort)opCode;
        }

        /// <summary>
        /// Build a final compressed, encrypted, and encoded packet.
        /// </summary>
        /// <returns>The packet data.</returns>
        public byte[] BuildPacket()
        {
            // Get the packet data
            byte[] data = Data.ToArray();

            // Compression
            bool compressed = false;
            if (DataLength > 100)
            {
                byte[] compressedData = CompressionService.Compress(data);
                if (compressedData.Length < data.Length)
                {
                    data = compressedData;
                    compressed = true;
                }
            }
            ushort dataLength = (ushort)data.Length;

            // Create the packet header
            InternalPacketHeader header = new InternalPacketHeader
            {
                OpCode = OpCode,
                Compressed = compressed,
                Length = dataLength
            };
            LastHeader = header;

            // Append the header and CRC to the compressed data
            ToffeePacket packet = new ToffeePacket(Sender);
            packet.Write(header);
            packet.Write(data, true);
            packet.Write(CRC.CalculateCRC32(data));
            return packet.GetBytes();
        }

        public static ToffeeInternalPacket Create<T>(ToffeeParticipant sender, ushort opCode, T o)
        {
            ToffeeInternalPacket packet = new ToffeeInternalPacket(sender, opCode);
            packet.WriteStruct(o);
            return packet;
        }

        public static ToffeeInternalPacketReadResult Read(ToffeeParticipant receiver, byte[] packet)
        {
            try
            {
                // Iterate through the packet
                ToffeePacketIterator iterator = new ToffeePacketIterator(receiver, packet);

                // Get the header, data, and CRC
                InternalPacketHeader header = iterator.ReadStruct<InternalPacketHeader>();
                byte[] packetData = iterator.ReadBytes(header.Length);
                uint sentCrc = iterator.ReadUInt32();

                // Is the CRC correct?
                uint calculatedCrc = CRC.CalculateCRC32(packetData);
                if (sentCrc != calculatedCrc)
                    return new ToffeeInternalPacketReadResult(false, new InternalPacketHeader(), null);

                // Is this packet compressed?
                if (header.Compressed)
                    packetData = CompressionService.Decompress(packetData);

                // Return the read result
                return new ToffeeInternalPacketReadResult(true, header, packetData);
            }
            catch
            {
                return new ToffeeInternalPacketReadResult(false, new InternalPacketHeader(), null);
            }
        }

        public static ToffeeInternalPacketReadResult Read<T>(ToffeeParticipant receiver, byte[] packet, out T output) where T : new()
        {
            // Default the output
            output = new T();

            // Read the packet
            ToffeeInternalPacketReadResult result = Read(receiver, packet);
            if (result.Success)
            {
                // Create a new iterator for the decompressed/decrypted packet data
                ToffeePacketIterator iterator = new ToffeePacketIterator(receiver, result.Data);
                output = (T)iterator.Read(typeof(T));
            }

            // Return the read result
            return result;
        }
    }
}
