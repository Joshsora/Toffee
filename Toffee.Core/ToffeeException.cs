using System;

namespace Toffee.Core
{
    [Serializable]
    public class ToffeeException : Exception
    {
        public ToffeeException() { }
        public ToffeeException(string message) : base(message) { }
        public ToffeeException(string message, params object[] args) : this(string.Format(message, args)) { }
        public ToffeeException(string message, Exception inner) : base(message, inner) { }
    }
}
