using System;

namespace Toffee.Protocol.Definitions.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ToffeeStructureAttribute : Attribute
    {
        public string NetworkName { get; private set; }

        public ToffeeStructureAttribute(string networkName)
        {
            NetworkName = networkName;
        }

        public ToffeeStructureAttribute() : this("") { }
    }
}
