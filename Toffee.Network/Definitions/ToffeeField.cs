using Toffee.Util;
using Toffee.Core.Packets;

namespace Toffee.Core.Definitions
{
    public abstract class ToffeeField
    {
        public ToffeeObject Owner { get; private set; }
        public string Identifier { get; private set; }
        public uint FieldId
        {
            get
            {
                return CRC.CalculateCRC32(Identifier);
            }
        }

        public bool IsMethod
        {
            get
            {
                return (GetType() == typeof(ToffeeMethod));
            }
        }

        public ToffeeMethod Method
        {
            get
            {
                if (IsMethod)
                    return (ToffeeMethod)this;
                return null;
            }
        }

        public bool IsProperty
        {
            get
            {
                return (GetType() == typeof(ToffeeProperty));
            }
        }

        public ToffeeProperty Property
        {
            get
            {
                if (IsProperty)
                    return (ToffeeProperty)this;
                return null;
            }
        }

        public ToffeeModifiers Modifiers { get; private set; }

        public ToffeeField(ToffeeObject owner, string identifier, ToffeeModifiers modifiers = ToffeeModifiers.None)
        {
            Owner = owner;
            Identifier = identifier;
            Modifiers = modifiers;
        }

        public abstract FieldUpdate MakeFieldUpdate(params object[] parameters);
    }
}
