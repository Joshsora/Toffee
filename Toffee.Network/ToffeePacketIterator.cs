using System;
using System.Text;
using System.Linq;
using Fasterflect;

using Toffee.Core.Definitions;

namespace Toffee.Core
{
    public class ToffeePacketIterator
    {
        public ToffeeNetwork Network { get; private set; }
        public byte[] Data { get; private set; }
        public int Position { get; set; }

        private ToffeePacketIterator(byte[] packet)
        {
            Data = packet;
            Position = 0;
        }

        public ToffeePacketIterator(ToffeeNetwork network, byte[] packet) : this(packet)
        {
            Network = network;
        }

        public ToffeePacketIterator(ToffeeParticipant receiver, byte[] packet) : this(packet)
        {
            Network = receiver.Network;
        }

        public byte[] ReadBytes(uint count)
        {
            if (Position + count > Data.Length)
                throw new IndexOutOfRangeException();
            byte[] bytes = new byte[count];
            for (int i = 0; i < count; i++)
                bytes[i] = ReadUInt8();
            return bytes;
        }

        public bool ReadBoolean()
        {
            return ReadUInt8() == 0x01;
        }

        public char ReadChar()
        {
            return (char)ReadUInt8();
        }

        public sbyte ReadInt8()
        {
            return (sbyte)ReadUInt8();
        }

        public short ReadInt16()
        {
            byte[] vBytes = ReadBytes(2);
            if (BitConverter.IsLittleEndian)
                vBytes = vBytes.Reverse().ToArray();
            return BitConverter.ToInt16(vBytes, 0);
        }

        public int ReadInt32()
        {
            byte[] vBytes = ReadBytes(4);
            if (BitConverter.IsLittleEndian)
                vBytes = vBytes.Reverse().ToArray();
            return BitConverter.ToInt32(vBytes, 0);
        }

        public long ReadInt64()
        {
            byte[] vBytes = ReadBytes(8);
            if (BitConverter.IsLittleEndian)
                vBytes = vBytes.Reverse().ToArray();
            return BitConverter.ToInt64(vBytes, 0);
        }

        public byte ReadUInt8()
        {
            return Data[Position++];
        }

        public ushort ReadUInt16()
        {
            byte[] vBytes = ReadBytes(2);
            if (BitConverter.IsLittleEndian)
                vBytes = vBytes.Reverse().ToArray();
            return BitConverter.ToUInt16(vBytes, 0);
        }

        public uint ReadUInt32()
        {
            byte[] vBytes = ReadBytes(4);
            if (BitConverter.IsLittleEndian)
                vBytes = vBytes.Reverse().ToArray();
            return BitConverter.ToUInt32(vBytes, 0);
        }

        public ulong ReadUInt64()
        {
            byte[] vBytes = ReadBytes(8);
            if (BitConverter.IsLittleEndian)
                vBytes = vBytes.Reverse().ToArray();
            return BitConverter.ToUInt64(vBytes, 0);
        }

        public string ReadString()
        {
            uint length = ReadUInt32();
            byte[] vBytes = ReadBytes(length);
            return Encoding.UTF8.GetString(vBytes);
        }

        public float ReadFloat32()
        {
            byte[] vBytes = ReadBytes(4);
            return BitConverter.ToSingle(vBytes, 0);
        }

        public double ReadFloat64()
        {
            byte[] vBytes = ReadBytes(8);
            return BitConverter.ToDouble(vBytes, 0);
        }

        public ToffeeValueType ReadValueType()
        {
            return (ToffeeValueType)ReadUInt8();
        }

        public Array ReadArray()
        {
            // Get the properties of the array
            uint length = ReadUInt32();
            ToffeeValueType elementVType = ReadValueType();
            uint structId = 0;
            if (elementVType == ToffeeValueType.Struct)
                structId = ReadUInt32();
            Type elementType = ToffeeType.GetTypeFromToffeeValueType(Network, elementVType, structId: structId);

            // Create the array and fill it
            Array array = Array.CreateInstance(elementType, length);
            for (int i = 0; i < length; i++)
                array.SetValue(Read(elementType), i);
            return array;
        }

        public T ReadStruct<T>() where T : new()
        {
            object o = new T();
            ToffeeStruct definition = Network.GetObject<ToffeeStruct>(o.GetType().FullName);
            if (definition == null)
                throw new Exception(string.Format("Cannot read Type '{0}' from ToffeePacket", o.GetType().Name));
            if (Position + definition.MinimumSize > Data.Length)
                throw new IndexOutOfRangeException();
            if (!definition.ImplementsInterface)
            {
                foreach (ToffeeProperty property in definition.Properties)
                    property.Set(o, Read(property.PropertyInfo.PropertyType));
            }
            else
                ((IToffeeStructure)o).ReadFrom(this);
            return (T)o;
        }

        public object Read(Type t)
        {
            TypeCode tCode = Type.GetTypeCode(t);
            switch (tCode)
            {
                case TypeCode.Boolean:
                    return ReadBoolean();
                case TypeCode.Char:
                    return ReadChar();
                case TypeCode.SByte:
                    return ReadInt8();
                case TypeCode.Int16:
                    return ReadInt16();
                case TypeCode.Int32:
                    return ReadInt32();
                case TypeCode.Int64:
                    return ReadInt64();
                case TypeCode.Byte:
                    return ReadUInt8();
                case TypeCode.UInt16:
                    return ReadUInt16();
                case TypeCode.UInt32:
                    return ReadUInt32();
                case TypeCode.UInt64:
                    return ReadUInt64();
                case TypeCode.String:
                    return ReadString();
                case TypeCode.Single:
                    return ReadFloat32();
                case TypeCode.Double:
                    return ReadFloat64();
                case TypeCode.Object:
                    if (t.IsArray)
                        return ReadArray();
                    else if (Network.HasObject<ToffeeStruct>(t.FullName))
                    {
                        var read = GetType().Method("ReadStruct").MakeGenericMethod(t);
                        return read.Invoke(this, null);
                    }
                    throw new Exception(string.Format("Cannot read Type '{0}' from ToffeePacket", t.Name));
                default:
                    throw new Exception(string.Format("Cannot read Type '{0}' from ToffeePacket", t.Name));
            }
        }
    }
}
