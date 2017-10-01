using System.Collections.Generic;
using Toffee.Compiler.Generator.Commands;

namespace Toffee.Compiler.Generator
{
    public class ToffeeGeneratorStackFrame
    {
        public string Name { get; set; }
        public List<IGeneratorCommand> Commands { get; set; }
        public object State { get; set; }

        public ToffeeGeneratorStackFrame(string name)
        {
            Name = name;
            Commands = new List<IGeneratorCommand>();
        }
    }
}
