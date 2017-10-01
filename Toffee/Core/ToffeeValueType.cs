namespace Toffee.Core
{
    /// <summary>
    /// An enum of all valid data types that may appear in a ToffeePacket.
    /// </summary>
    public enum ToffeeValueType
    {
        None,

        /// <summary>
        /// Boolean.
        /// </summary>
        Bool,

        /// <summary>
        /// Char.
        /// </summary>
        Char,

        /// <summary>
        /// Signed byte.
        /// </summary>
        Int8,

        /// <summary>
        /// Signed short.
        /// </summary>
        Int16,

        /// <summary>
        /// Signed integer.
        /// </summary>
        Int32,

        /// <summary>
        /// Signed long.
        /// </summary>
        Int64,

        /// <summary>
        /// Unsigned byte.
        /// </summary>
        UInt8,
        
        /// <summary>
        /// Unsigned short.
        /// </summary>
        UInt16,
        
        /// <summary>
        /// Unsigned integer.
        /// </summary>
        UInt32,

        /// <summary>
        /// Unsigned long.
        /// </summary>
        UInt64,

        /// <summary>
        /// String.
        /// </summary>
        String,

        /// <summary>
        /// Array.
        /// </summary>
        Array,

        /// <summary>
        /// Float.
        /// </summary>
        Float32,

        /// <summary>
        /// Double.
        /// </summary>
        Float64,

        /// <summary>
        /// Any valid ToffeeStructure.
        /// </summary>
        Struct
    }
}
