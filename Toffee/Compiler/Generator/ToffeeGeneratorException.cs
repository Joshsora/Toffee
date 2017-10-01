using System;

namespace Toffee.Compiler.Generator
{
    [Serializable]
    public class ToffeeGeneratorException : Exception
    {
        public ToffeeGeneratorException() { }
        public ToffeeGeneratorException(string message) : base(message) { }
        public ToffeeGeneratorException(string message, params object[] args) : this(string.Format(message, args)) { }
        public ToffeeGeneratorException(string message, Exception inner) : base(message, inner) { }
    }
}
