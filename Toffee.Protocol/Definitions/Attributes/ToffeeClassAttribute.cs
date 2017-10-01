using System;

namespace Toffee.Core.Definitions.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ToffeeClassAttribute : Attribute
    {
        public string NetworkName { get; private set; }

        public ToffeeClassAttribute(string networkName)
        {
            NetworkName = networkName;
        }

        public ToffeeClassAttribute() : this("") { }
    }
}
