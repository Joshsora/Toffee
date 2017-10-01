using Toffee.Compiler.Writer;

namespace Toffee.Compiler.Generator.Commands
{
    public class DataCommand : IGeneratorCommand
    {
        public string WantedData { get; private set; }

        public DataCommand(string wantedData)
        {
            WantedData = wantedData;
        }

        public void Execute(ToffeeWriter writer)
        {
            writer.GetData(WantedData);
        }

        [GeneratorCommandParser("data")]
        public static void Parse(ToffeeGenerator generator, string[] args)
        {
            if (args.Length != 1)
                throw new ToffeeGeneratorException("Invalid number of arguments for 'data' command.");
            generator.AddCommand(new DataCommand(args[0]));
        }
    }
}
