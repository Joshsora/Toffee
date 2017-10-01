using Toffee.Compiler.Writer;

namespace Toffee.Compiler.Generator.Commands
{
    public class WriteCommand : IGeneratorCommand
    {
        public ToffeeGenerator Generator { get; private set; }
        private string Value { get; set; }

        public WriteCommand(ToffeeGenerator generator, string value)
        {
            Generator = generator;
            Value = value;

            generator.Log("Write({0})", value);
        }

        public void Execute(ToffeeWriter writer)
        {
            writer.Log("Writing {0}", Value);
            string value = writer.GetLineToWrite(Value);
            if (writer.AppendNewline)
                writer.Writer.WriteLine(value);
            else
                writer.Writer.Write(value);
        }
    }
}
