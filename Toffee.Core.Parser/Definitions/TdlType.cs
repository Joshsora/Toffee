namespace Toffee.Core.Parser.Definitions
{
    public class TdlType
    {
        public ToffeeValueType Type { get; private set; }
        public bool Array { get; private set; }
        public TdlStruct StructType { get; private set; }
        public uint MinimumSize
        {
            get
            {
                if (Array)
                    return 4;
                switch (Type)
                {
                    case ToffeeValueType.Bool:
                        return 1;

                    case ToffeeValueType.Char:
                        return 1;

                    case ToffeeValueType.Int8:
                        return 1;

                    case ToffeeValueType.UInt8:
                        return 1;

                    case ToffeeValueType.Int16:
                        return 2;

                    case ToffeeValueType.UInt16:
                        return 2;

                    case ToffeeValueType.Int32:
                        return 4;

                    case ToffeeValueType.UInt32:
                        return 4;

                    case ToffeeValueType.Int64:
                        return 8;

                    case ToffeeValueType.UInt64:
                        return 8;

                    case ToffeeValueType.String:
                        return 4;

                    case ToffeeValueType.Float32:
                        return 4;

                    case ToffeeValueType.Float64:
                        return 8;

                    case ToffeeValueType.Struct:
                        return StructType.MinimumSize;

                    default:
                        return 0;
                }
            }
        }

        public string ReadMethod
        {
            get
            {
                if (Array)
                    return "ReadArray";
                switch (Type)
                {
                    case ToffeeValueType.Bool:
                        return "ReadBoolean";

                    case ToffeeValueType.Char:
                        return "ReadChar";

                    case ToffeeValueType.Int8:
                        return "ReadInt8";

                    case ToffeeValueType.UInt8:
                        return "ReadUInt8";

                    case ToffeeValueType.Int16:
                        return "ReadInt16";

                    case ToffeeValueType.UInt16:
                        return "ReadUInt16";

                    case ToffeeValueType.Int32:
                        return "ReadInt32";

                    case ToffeeValueType.UInt32:
                        return "ReadUInt32";

                    case ToffeeValueType.Int64:
                        return "ReadInt64";

                    case ToffeeValueType.UInt64:
                        return "ReadUInt64";

                    case ToffeeValueType.String:
                        return "ReadString";

                    case ToffeeValueType.Float32:
                        return "ReadFloat32";

                    case ToffeeValueType.Float64:
                        return "ReadFloat64";

                    case ToffeeValueType.Struct:
                        
                        return "ReadStruct<" + StructType.Identifier + ">";

                    default:
                        return "Read";
                }
            }
        }

        public TdlType(ToffeeValueType type, bool array = false)
        {
            Type = type;
            Array = array;
            StructType = null;
        }

        public TdlType(ToffeeValueType type, TdlStruct structType, bool array = false)
            : this(type, array)
        {
            StructType = structType;
        }

        public override string ToString()
        {
            string returnVal = "";
            switch (Type)
            {
                case ToffeeValueType.Bool:
                    returnVal = "bool";
                    break;

                case ToffeeValueType.Char:
                    returnVal = "char";
                    break;

                case ToffeeValueType.Int8:
                    returnVal = "sbyte";
                    break;

                case ToffeeValueType.UInt8:
                    returnVal = "byte";
                    break;

                case ToffeeValueType.Int16:
                    returnVal = "short";
                    break;

                case ToffeeValueType.UInt16:
                    returnVal = "ushort";
                    break;

                case ToffeeValueType.Int32:
                    returnVal = "int";
                    break;

                case ToffeeValueType.UInt32:
                    returnVal = "uint";
                    break;

                case ToffeeValueType.Int64:
                    returnVal = "long";
                    break;

                case ToffeeValueType.UInt64:
                    returnVal = "ulong";
                    break;

                case ToffeeValueType.String:
                    returnVal = "string";
                    break;

                case ToffeeValueType.Float32:
                    returnVal = "float";
                    break;

                case ToffeeValueType.Float64:
                    returnVal = "double";
                    break;

                case ToffeeValueType.Struct:
                    returnVal = StructType.Identifier;
                    break;
            }
            if (Array)
                returnVal += "[]";
            return returnVal;
        }
    }
}
