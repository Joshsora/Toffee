using System;

namespace Toffee.Core.Definitions.Attributes
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class ToffeeServiceAttribute : Attribute
    {
        public string NetworkName { get; private set; }

        public ToffeeServiceAttribute(string networkName)
        {
            NetworkName = networkName;
        }

        public ToffeeServiceAttribute() : this("") { }
    }
}
