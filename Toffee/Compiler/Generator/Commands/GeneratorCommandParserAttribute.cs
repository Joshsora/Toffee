using System;

namespace Toffee.Compiler.Generator.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GeneratorCommandParserAttribute : Attribute
    {
        public string Keyword { get; private set; }

        public GeneratorCommandParserAttribute(string keyword)
        {
            Keyword = keyword;
        }
    }
}
