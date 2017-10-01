using System;

namespace Toffee.Compiler.Generator
{
    [Serializable]
    public class ToffeeWriterException : Exception
    {
        public ToffeeWriterException() { }
        public ToffeeWriterException(string message) : base(message) { }
        public ToffeeWriterException(string message, params object[] args) : this(string.Format(message, args)) { }
        public ToffeeWriterException(string message, Exception inner) : base(message, inner) { }
    }
}
