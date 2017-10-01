using Toffee.Util;

namespace Toffee.Core.Parser.Definitions
{
    public class TdlField
    {
        public string Identifier { get; private set; }
        public TdlObject Owner { get; private set; }
        public uint FieldId { get; private set; }
        public ToffeeModifiers Modifiers { get; set; }

        public TdlField(string identifier, TdlObject owner, ToffeeModifiers modifiers = ToffeeModifiers.None)
        {
            Identifier = identifier;
            Owner = owner;
            FieldId = CRC.CalculateCRC32(Identifier);
            Modifiers = modifiers;
            Owner.File.AddStringToHash(Identifier);
        }

        public bool HasModifier(ToffeeModifiers modifier)
        {
            return (Modifiers & modifier) == modifier;
        }
    }
}
