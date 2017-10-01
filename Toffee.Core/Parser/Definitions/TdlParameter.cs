namespace Toffee.Core.Parser.Definitions
{
    public class TdlParameter
    {
        public string Identifier { get; private set; }
        public TdlType Type { get; private set; }

        public TdlParameter(string identifier, TdlType type)
        {
            Identifier = identifier;
            Type = type;
        }
    }
}
