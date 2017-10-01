using System;

namespace Toffee.Core.Definitions.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ToffeeStructurePropertyAttribute : Attribute
    {
        public int Position { get; private set; }

        public ToffeeStructurePropertyAttribute(int position)
        {
            Position = position;
        }
    }
}
