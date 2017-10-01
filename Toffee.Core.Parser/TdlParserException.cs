using System;

namespace Toffee.Core.Parser
{
    [Serializable]
    public class TdlParserException : Exception
    {
        public TdlParserException() { }
        public TdlParserException(string message) : base(message) { }
        public TdlParserException(string message, params object[] args) : this(string.Format(message, args)) { }
        public TdlParserException(string message, Exception inner) : base(message, inner) { }
    }
}
