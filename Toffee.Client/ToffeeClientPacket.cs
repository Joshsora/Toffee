using System.Linq;
using Toffee.Core;
using Toffee.Protocol;
using Toffee.Protocol.Packets.Client;
using Toffee.Util;

namespace Toffee.Client
{
    public class ToffeeClientPacket : ToffeePacket
    {
        /// <summary>
        /// The OpCode of the packet.
        /// </summary>
        public ushort OpCode { get; private set; }

        /// <summary>
        /// The Header that was created for this packet last time 'BuildPacket' was called.
        /// </summary>
        public ClientPacketHeader LastHeader { get; private set; }

        public ToffeeClientPacket(ToffeeParticipant client, ushort opCode) : base(client)
        {
            OpCode = opCode;
        }

        public ToffeeClientPacket(ToffeeParticipant client, ToffeeOpCode opCode) : base(client)
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
            ClientPacketHeader header = new ClientPacketHeader
            {
                OpCode = OpCode,
                Compressed = compressed,
                Length = dataLength,
                Timestamp = TimeUtil.GetUnixTimestamp()
            };
            LastHeader = header;

            // Append the header and CRC to the compressed data
            ToffeePacket packet = new ToffeePacket(Sender);
            packet.Write(header);
            packet.Write(data, true);
            packet.Write(CRC.CalculateCRC32(data));
            data = packet.GetBytes();

            // Encryption
            bool encrypted = ((Encrypted && Sender.UseEncryption) || (Sender.ForceEncryption));
            if ((encrypted) && (!Sender.UseEncryption))
            {
                Sender.Disconnect();
                return new byte[0];
            }
            if (encrypted)
                data = Sender.Encryption.Encrypt(data);

            // Create the final packet
            packet = new ToffeePacket(Sender);
            packet.Write(encrypted);
            packet.Write(data, true);
            return packet.GetBytes();
        }

        public static ToffeeClientPacket Create<T>(ToffeeParticipant sender, ushort opCode, T o)
        {
            ToffeeClientPacket packet = new ToffeeClientPacket(sender, opCode);
            packet.WriteStruct(o);
            return packet;
        }

        public static ToffeeClientPacketReadResult Read(ToffeeParticipant receiver, byte[] packet)
        {
            try
            {
                // Is this packet encrypted?
                bool encrypted = packet[0] == 0x01;
                if (encrypted)
                {
                    // Is this client setup to use encryption?
                    if (receiver.UseEncryption)
                        packet = receiver.Encryption.Decrypt(packet.Skip(1).ToArray());
                }
                else
                    packet = packet.Skip(1).ToArray();

                // Iterate through the packet
                ToffeePacketIterator iterator = new ToffeePacketIterator(receiver, packet);

                // Get the header, data, and CRC
                ClientPacketHeader header = iterator.ReadStruct<ClientPacketHeader>();
                byte[] packetData = iterator.ReadBytes(header.Length);
                uint sentCrc = iterator.ReadUInt32();

                // Is the CRC correct?
                uint calculatedCrc = CRC.CalculateCRC32(packetData);
                if (sentCrc != calculatedCrc)
                    return new ToffeeClientPacketReadResult(false, encrypted, new ClientPacketHeader(), null);

                // Is this packet compressed?
                if (header.Compressed)
                    packetData = CompressionService.Decompress(packetData);

                // Return the read result
                return new ToffeeClientPacketReadResult(true, encrypted, header, packetData);
            }
            catch
            {
                return new ToffeeClientPacketReadResult(false, false, new ClientPacketHeader(), null);
            }
        }

        public static ToffeeClientPacketReadResult Read<T>(ToffeeParticipant receiver, byte[] packet, out T output) where T : new()
        {
            // Default the output
            output = new T();

            // Read the packet
            ToffeeClientPacketReadResult result = Read(receiver, packet);
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
