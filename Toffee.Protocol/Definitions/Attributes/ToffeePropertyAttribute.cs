using System;
using Toffee.Core;

namespace Toffee.Protocol.Definitions.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ToffeePropertyAttribute : Attribute
    {
        public ToffeeModifiers Modifiers { get; private set; }

        public ToffeePropertyAttribute(ToffeeModifiers modifiers)
        {
            Modifiers = modifiers;
        }

        public ToffeePropertyAttribute() : this(ToffeeModifiers.ClientSend) { }
    }
}
