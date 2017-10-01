using Toffee.Compiler.Writer;

namespace Toffee.Compiler.Generator.Commands
{
    public class RepeatCommand : IGeneratorCommand
    {
        public void Execute(ToffeeWriter writer)
        {
            writer.Repeat();
        }

        [GeneratorCommandParser("repeat")]
        public static void Parse(ToffeeGenerator generator, string[] args)
        {
            // Do we have the correct arguments?
            if (args.Length != 0)
                throw new ToffeeGeneratorException("Invalid number of arguments for 'repeat' command.");
            generator.AddCommand(new RepeatCommand());
        }
    }
}
