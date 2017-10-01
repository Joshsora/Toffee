using System;

namespace Toffee.Core.Parser.Definitions
{
    public class TdlProperty : TdlField
    {
        public TdlType Type { get; private set; }

        public TdlProperty(string identifier, TdlObject owner, TdlType type, 
            ToffeeModifiers modifiers = ToffeeModifiers.None) : base(identifier, owner, modifiers)
        {
            if (owner.GetType() == typeof(TdlService))
                throw new Exception("TdlProperty cannot be owned by TdlService.");
            Type = type;
        }
    }
}
