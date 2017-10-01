using System.IO;
using Toffee.Compiler.Writer;
using Toffee.Compiler.Generator.Commands;

namespace Toffee.Compiler.Generator
{
    public class ToffeeWriterStackFrame
    {
        public string Name { get; private set; }
        public IGeneratorCommand[] Commands { get; private set; }
        private Stream _Stream;
        public Stream Stream
        {
            get
            {
                return _Stream;
            }

            set
            {
                if (_Stream != null)
                    _Stream.Dispose();
                _Stream = value;
                Writer = new StreamWriter(_Stream);
            }
        }
        private StreamWriter _Writer;
        public StreamWriter Writer
        {
            get
            {
                return _Writer;
            }

            set
            {
                _Writer = value;
            }
        }

        public ToffeeWriterStackFrame(string name, IGeneratorCommand[] commands)
        {
            Name = name;
            Commands = commands;
        }

        public void Execute(ToffeeWriter writer)
        {
            foreach (IGeneratorCommand command in Commands)
                command.Execute(writer);
        }
    }
}
