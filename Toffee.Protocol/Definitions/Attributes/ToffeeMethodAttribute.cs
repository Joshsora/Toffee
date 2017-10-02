using System;
using Toffee.Core;

namespace Toffee.Protocol.Definitions.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ToffeeMethodAttribute : Attribute
    {
        public ToffeeModifiers Modifiers { get; private set; }

        public ToffeeMethodAttribute(ToffeeModifiers modifiers)
        {
            Modifiers = modifiers;
        }

        public ToffeeMethodAttribute() : this(ToffeeModifiers.ClientSend) { }
    }
}
