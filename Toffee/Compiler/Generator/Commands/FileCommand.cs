using System.IO;
using Toffee.Compiler.Writer;

namespace Toffee.Compiler.Generator.Commands
{
    public class FileCommand : IGeneratorCommand
    {
        public string Command { get; private set; }
        public string Argument { get; private set; }

        public FileCommand(string command, string arg="")
        {
            Command = command;
            Argument = arg;
        }

        public void Execute(ToffeeWriter writer)
        {
            if (Command == "begin")
            {
                string filename = writer.GetLineToWrite(Argument);
                string filepath;
                if (writer.OutputDirectory != "")
                    filepath = Path.Combine(writer.OutputDirectory, filename);
                else
                    filepath = Path.GetFullPath(filename);
                string path = Path.GetDirectoryName(filepath);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                writer.Log("Begin file: {0}", filepath);
                writer.Stream = new FileStream(filepath, FileMode.Create);
            }
            else if (Command == "end")
            {
                writer.Writer.Flush();
                writer.Stream.Dispose();
            }
            else
                writer.Log("Unknown file command '{0}'", Command);
        }

        [GeneratorCommandParser("file")]
        public static void Parse(ToffeeGenerator generator, string[] args)
        {
            // Do we have the correct arguments?
            if ((args.Length == 0) || (args.Length > 2))
                throw new ToffeeGeneratorException("Invalid number of arguments for 'writer' command.");
            FileCommand command;
            if (args.Length == 1)
            {
                command = new FileCommand(args[0]);
                generator.Log("File('{0}')", args[0]);
            }
            else
            {
                command = new FileCommand(args[0], args[1]);
                generator.Log("File('{0}', '{1}')", args[0], args[1]);
            }
            generator.AddCommand(command);
        }
    }
}
