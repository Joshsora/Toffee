using System;

namespace Toffee.Protocol.Definitions.Attributes
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
