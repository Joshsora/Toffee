using System;

namespace Toffee.Server.Roles
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ToffeeRoleAttribute : Attribute
    {
        public string Name { get; private set; }

        public ToffeeRoleAttribute(string name)
        {
            Name = name;
        }
    }
}
