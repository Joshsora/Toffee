using System;

namespace Toffee.Compiler.Writer
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ToffeeDataGathererAttribute : Attribute
    {
        public string ScriptName { get; private set; }

        public ToffeeDataGathererAttribute(string scriptName)
        {
            ScriptName = scriptName;
        }
    }
}
