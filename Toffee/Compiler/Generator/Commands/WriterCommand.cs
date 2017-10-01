using Toffee.Compiler.Writer;

namespace Toffee.Compiler.Generator.Commands
{
    public class WriterCommand : IGeneratorCommand
    {
        public string Command { get; private set; }
        public string Argument { get; private set; }

        public WriterCommand(string command, string argument)
        {
            Command = command;
            Argument = argument;
        }

        public void Execute(ToffeeWriter writer)
        {
            switch (Command.ToLower())
            {
                case "indent":
                    string argument = Argument.ToLower();
                    if (argument == "forwards")
                        writer.IndentLevel++;
                    else if (argument == "backwards")
                        writer.IndentLevel--;
                    else if (argument == "on")
                        writer.Indent = true;
                    else if (argument == "off")
                        writer.Indent = false;
                    break;

                case "newline":
                    if (Argument == "0")
                        writer.AppendNewline = false;
                    else
                        writer.AppendNewline = true;
                    break;

                case "crash":
                    throw new ToffeeWriterException("Writer was told to crash.");

                default:
                    writer.Log("Unknown writer command '{0}'", Command);
                    break;
            }
        }

        [GeneratorCommandParser("writer")]
        public static void Parse(ToffeeGenerator generator, string[] args)
        {
            // Do we have the correct arguments?
            if (args.Length != 2)
                throw new ToffeeGeneratorException("Invalid number of arguments for 'writer' command.");
            generator.Log("Writer('{0}', '{1}')", args[0], args[1]);
            WriterCommand command = new WriterCommand(args[0], args[1]);
            generator.AddCommand(command);
        }
    }
}
