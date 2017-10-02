using System;
using Toffee.Core;
using Toffee.Protocol.Definitions;

namespace Toffee.Protocol
{
    public class ToffeeType
    {
        public ToffeeValueType BaseType { get; private set; }
        public ToffeeType SubType { get; private set; }
        public uint StructId { get; private set; }

        public ToffeeType(ToffeeValueType baseType)
        {
            BaseType = baseType;
            if (BaseType == ToffeeValueType.Array)
                SubType = new ToffeeType(ToffeeValueType.UInt8);
            else
                SubType = new ToffeeType(ToffeeValueType.None);
        }

        public ToffeeType(ToffeeValueType subType, bool array)
        {
            BaseType = ToffeeValueType.Array;
            SubType = new ToffeeType(subType);
        }

        public ToffeeType(ToffeeNetwork network, uint structId)
        {
            if (network.GetObject<ToffeeStruct>(structId) == null)
                throw new Exception("Cannot make a ToffeeType instance with invalid structId");
            BaseType = ToffeeValueType.Struct;
            SubType = new ToffeeType(ToffeeValueType.None);
            StructId = structId;
        }

        public ToffeeType(ToffeeNetwork network, Type t)
        {
            BaseType = GetToffeeValueTypeFromType(network, t);
            if (t.IsArray)
                SubType = new ToffeeType(network, t.GetElementType());
            else if ((t.IsValueType) && (network.HasObject<ToffeeStruct>(t.FullName)))
                StructId = network.GetObject<ToffeeStruct>(t.FullName).ObjectId;
        }

        public ToffeeType(ToffeeType subType)
        {
            BaseType = ToffeeValueType.Array;
            SubType = subType;
        }

        public bool TypeMatches(ToffeeNetwork network, object o)
        {
            Type t = o.GetType();
            ToffeeValueType toffeeType = GetToffeeValueTypeFromType(network, t);
            if (toffeeType == BaseType)
            {
                if ((toffeeType != ToffeeValueType.Array) && (toffeeType != ToffeeValueType.Struct))
                    return true;
                else
                {
                    if (t.IsArray)
                        return SubType.TypeMatches(network, t.GetElementType());
                    else if ((t.IsValueType) && (network.HasObject<ToffeeStruct>(t.FullName)))
                        return StructId == network.GetObject<ToffeeStruct>(t.FullName).ObjectId;
                }
            }

            return false;
        }

        public static Type GetTypeFromToffeeValueType(ToffeeNetwork network, ToffeeValueType type, 
            ToffeeValueType subType = ToffeeValueType.UInt8, uint structId = 0, uint length = 0)
        {
            switch (type)
            {
                case ToffeeValueType.Bool:
                    return typeof(bool);
                case ToffeeValueType.Char:
                    return typeof(char);
                case ToffeeValueType.Int8:
                    return typeof(sbyte);
                case ToffeeValueType.Int16:
                    return typeof(short);
                case ToffeeValueType.Int32:
                    return typeof(int);
                case ToffeeValueType.Int64:
                    return typeof(long);
                case ToffeeValueType.UInt8:
                    return typeof(byte);
                case ToffeeValueType.UInt16:
                    return typeof(ushort);
                case ToffeeValueType.UInt32:
                    return typeof(uint);
                case ToffeeValueType.UInt64:
                    return typeof(ulong);
                case ToffeeValueType.String:
                    return typeof(string);
                case ToffeeValueType.Float32:
                    return typeof(float);
                case ToffeeValueType.Float64:
                    return typeof(double);
                case ToffeeValueType.Array:
                    return Array.CreateInstance(GetTypeFromToffeeValueType(network, subType, structId: structId), length).GetType();
                case ToffeeValueType.Struct:
                    ToffeeStruct definition = network.GetObject<ToffeeStruct>(structId);
                    if (definition == null)
                        throw new Exception(string.Format("Cannot get a Type instance from invalid structId: {0}", structId));
                    return definition.Type;
                default:
                    throw new Exception(string.Format("Cannot get a Type instance from {0}", type));
            }
        }

        public static ToffeeValueType GetToffeeValueTypeFromType(ToffeeNetwork network, Type t)
        {
            TypeCode tCode = Type.GetTypeCode(t);
            switch (tCode)
            {
                case TypeCode.Boolean:
                    return ToffeeValueType.Bool;
                case TypeCode.Char:
                    return ToffeeValueType.Char;
                case TypeCode.SByte:
                    return ToffeeValueType.Int8;
                case TypeCode.Int16:
                    return ToffeeValueType.Int16;
                case TypeCode.Int32:
                    return ToffeeValueType.Int32;
                case TypeCode.Int64:
                    return ToffeeValueType.Int64;
                case TypeCode.Byte:
                    return ToffeeValueType.UInt8;
                case TypeCode.UInt16:
                    return ToffeeValueType.UInt16;
                case TypeCode.UInt32:
                    return ToffeeValueType.UInt32;
                case TypeCode.UInt64:
                    return ToffeeValueType.UInt64;
                case TypeCode.String:
                    return ToffeeValueType.String;
                case TypeCode.Single:
                    return ToffeeValueType.Float32;
                case TypeCode.Double:
                    return ToffeeValueType.Float64;
                case TypeCode.Object:
                    if (t.IsArray)
                        return ToffeeValueType.Array;
                    else if (network.HasObject<ToffeeStruct>(t.FullName))
                        return ToffeeValueType.Struct;
                    else
                        throw new Exception(string.Format("{0} cannot be converted to a ToffeeValueType.", t));
                default:
                    throw new Exception(string.Format("{0} cannot be converted to a ToffeeValueType.", t));
            }
        }
    }
}
