using System;

namespace Toffee.Core.Definitions.Attributes
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
