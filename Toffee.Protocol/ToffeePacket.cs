using System;
using System.Text;
using System.Collections.Generic;
using Fasterflect;

using Toffee.Core;
using Toffee.Protocol.Definitions;

namespace Toffee.Protocol
{
    /// <summary>
    /// A class that represents a packet that is under construction.
    /// Used to write packets.
    /// </summary>
    public class ToffeePacket
    {
        /// <summary>
        /// The ToffeeNetwork containing all usable structures.
        /// </summary>
        public ToffeeNetwork Network { get; private set; }

        /// <summary>
        /// The ToffeeParticipant that is sending this packet.
        /// </summary>
        public ToffeeParticipant Sender { get; private set; }

        /// <summary>
        /// The packet data.
        /// </summary>
        protected List<byte> Data { get; set; }

        /// <summary>
        /// The length of the current packet data.
        /// </summary>
        protected int DataLength
        {
            get
            {
                return Data.Count;
            }
        }

        /// <summary>
        /// True if encryption should be considered, and false otherwise.
        /// (Defaults to false)
        /// </summary>
        public bool Encrypted { get; set; }

        private ToffeePacket()
        {
            Data = new List<byte>();
            Encrypted = false;
        }

        public ToffeePacket(ToffeeNetwork network) : this()
        {
            Network = network;
        }

        public ToffeePacket(ToffeeParticipant sender) : this()
        {
            Network = sender.Network;
            Sender = sender;
        }

        /// <summary>
        /// Write a boolean value.
        /// </summary>
        /// <param name="v">The value to write</param>
        public void Write(bool v)
        {
            Data.Add((byte)(v ? 1 : 0));
        }

        /// <summary>
        /// Write an unsigned byte value.
        /// </summary>
        /// <param name="v">The value to write</param>
        public void Write(byte v)
        {
            Data.Add(v);
        }

        /// <summary>
        /// Write a 7-bit character value.
        /// </summary>
        /// <param name="v">The value to write</param>
        public void Write(char v)
        {
            Write((byte)(v & 0x7F));
        }

        /// <summary>
        /// Write a signed short value.
        /// </summary>
        /// <param name="v">The value to write</param>
        public void Write(short v)
        {
            byte[] vBytes = BitConverter.GetBytes(v);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(vBytes);
            foreach (byte b in vBytes)
                Write(b);
        }

        /// <summary>
        /// Write a signed integer value.
        /// </summary>
        /// <param name="v">The value to write</param>
        public void Write(int v)
        {
            byte[] vBytes = BitConverter.GetBytes(v);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(vBytes);
            foreach (byte b in vBytes)
                Write(b);
        }

        /// <summary>
        /// Write a signed long value.
        /// </summary>
        /// <param name="v">The value to write</param>
        public void Write(long v)
        {
            byte[] vBytes = BitConverter.GetBytes(v);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(vBytes);
            foreach (byte b in vBytes)
                Write(b);
        }

        /// <summary>
        /// Write an unsigned short value.
        /// </summary>
        /// <param name="v">The value to write</param>
        public void Write(ushort v)
        {
            byte[] vBytes = BitConverter.GetBytes(v);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(vBytes);
            foreach (byte b in vBytes)
                Write(b);
        }

        /// <summary>
        /// Write an unsigned integer value.
        /// </summary>
        /// <param name="v">The value to write</param>
        public void Write(uint v)
        {
            byte[] vBytes = BitConverter.GetBytes(v);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(vBytes);
            foreach (byte b in vBytes)
                Write(b);
        }

        /// <summary>
        /// Write an unsigned long value.
        /// </summary>
        /// <param name="v">The value to write</param>
        public void Write(ulong v)
        {
            byte[] vBytes = BitConverter.GetBytes(v);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(vBytes);
            foreach (byte b in vBytes)
                Write(b);
        }

        /// <summary>
        /// Write a length-prefixed UTF8 string.
        /// </summary>
        /// <param name="v">The value to write</param>
        public void Write(string v)
        {
            if (v == null)
                v = "";
            Write(v.Length);
            foreach (byte b in Encoding.UTF8.GetBytes(v))
                Write(b);
        }

        /// <summary>
        /// Write an IEEE 754 4-byte floating point value.
        /// </summary>
        /// <param name="v">The value to write</param>
        public void Write(float v)
        {
            byte[] vBytes = BitConverter.GetBytes(v);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(vBytes);
            foreach (byte b in vBytes)
                Write(b);
        }

        /// <summary>
        /// Write an IEEE 754 8-byte floating point value.
        /// </summary>
        /// <param name="v">The value to write</param>
        public void Write(double v)
        {
            byte[] vBytes = BitConverter.GetBytes(v);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(vBytes);
            foreach (byte b in vBytes)
                Write(b);
        }

        /// <summary>
        /// Write an object. (determined at runtime)
        /// </summary>
        /// <param name="v">The value to write</param>
        public void Write(object o)
        {
            Type t = o.GetType();
            TypeCode tCode = Type.GetTypeCode(t);

            switch (tCode)
            {
                case TypeCode.Boolean:
                    Write((bool)o);
                    break;
                case TypeCode.Char:
                    Write((char)o);
                    break;
                case TypeCode.SByte:
                    Write((sbyte)o);
                    break;
                case TypeCode.Int16:
                    Write((short)o);
                    break;
                case TypeCode.Int32:
                    Write((int)o);
                    break;
                case TypeCode.Int64:
                    Write((long)o);
                    break;
                case TypeCode.Byte:
                    Write((byte)o);
                    break;
                case TypeCode.UInt16:
                    Write((ushort)o);
                    break;
                case TypeCode.UInt32:
                    Write((uint)o);
                    break;
                case TypeCode.UInt64:
                    Write((ulong)o);
                    break;
                case TypeCode.String:
                    Write((string)o);
                    break;
                case TypeCode.Single:
                    Write((float)o);
                    break;
                case TypeCode.Double:
                    Write((double)o);
                    break;
                case TypeCode.Object:
                    if (t.IsArray)
                        Write((Array)o);
                    else if (Network.HasObject<ToffeeStruct>(t.FullName))
                    {
                        var write = GetType().Method("WriteStruct").MakeGenericMethod(t);
                        write.Invoke(this, new object[] { o });
                    }
                    else
                        throw new Exception(string.Format("Cannot add Type '{0}' to ToffeePacket", t));
                    break;
                default:
                    throw new Exception(string.Format("Cannot add Type '{0}' to ToffeePacket", t));
            }
        }

        /// <summary>
        /// Write an array.
        /// </summary>
        /// <param name="v">The value to write</param>
        /// <param name="raw">If true, a length is not prefixed</param>
        public void Write(Array v, bool raw = false)
        {
            if (!raw)
            {
                Write(v.Length);
                Type t = v.GetType().GetElementType();
                Write(t);
            }
            foreach (object o in v)
                Write(o);
        }

        /// <summary>
        /// Write's a set of ToffeeValueType values to signify a Type.
        /// </summary>
        /// <param name="t">The type to write</param>
        public void Write(Type t)
        {
            ToffeeValueType toffeeType = ToffeeType.GetToffeeValueTypeFromType(Network, t);
            Write((byte)toffeeType);
            if (toffeeType == ToffeeValueType.Array)
                Write(t.GetElementType());
            if (toffeeType == ToffeeValueType.Struct)
                Write(Network.GetObject<ToffeeStruct>(t.FullName).ObjectId);
        }

        /// <summary>
        /// Write's a ToffeeStructure that is part of the sender's network.
        /// </summary>
        /// <typeparam name="T">The type of structure to write</typeparam>
        /// <param name="o">An instance of the type of structure</param>
        public void WriteStruct<T>(T o)
        {
            Type oT = typeof(T);
            if (!Network.HasObject<ToffeeStruct>(oT.FullName))
                throw new Exception(string.Format("Cannot add Type '{0}' to ToffeePacket", oT));
            ToffeeStruct definition = Network.GetObject<ToffeeStruct>(oT.FullName);
            if (!definition.ImplementsInterface)
            {
                foreach (var property in definition.Properties)
                    Write(property.Get(o));
            }
            else
                ((IToffeeStructure)o).WriteTo(this);
        }

        /// <summary>
        /// Get the current bytes of the packet.
        /// </summary>
        /// <returns>The packet data as a byte array.</returns>
        public byte[] GetBytes()
        {
            return Data.ToArray();
        }
    }
}
