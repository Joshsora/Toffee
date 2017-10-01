using System;
using System.IO;
using System.Diagnostics;
using Ookii.CommandLine;

using Toffee.Compiler.Generator;
using Toffee.Compiler.Writer;
using Toffee.Core.Parser;
using Toffee.Core.Parser.Definitions;

namespace ToffeeCompiler
{
    public class Program
    {
        public static Stopwatch Stopwatch { get; set; }
        public static ToffeeCompilerArguments Arguments { get; set; }
        public static ToffeeGenerator Generator { get; set; }
        public static ToffeeWriter Writer { get; set; }

        public static void Main(string[] args)
        {
            Stopwatch = new Stopwatch();
            Stopwatch.Start();

#if DEBUG
            if (args.Length == 0)
            {
                // Right, we're in DEBUG, let's make life easier and give some arguments
                args = new string[]
                {
                "csharp",
                "Example/Test/Test.tdl",
                "-OutputDirectory",
                "Built",
                "-Verbose"
                };
            }
#endif

            // First, let's parse the arguments we have
            CommandLineParser argsParser = new CommandLineParser(typeof(ToffeeCompilerArguments));
            try
            {
                Arguments = (ToffeeCompilerArguments)argsParser.Parse(args);
            }
            catch (CommandLineArgumentException e)
            {
                Console.WriteLine(e.Message);
                argsParser.WriteUsageToConsole();
                return;
            }

            // Next, let's load plugins
            // TODO

            // Now, let's run the application!
            if (BeginApplication())
            {
                Stopwatch.Stop();
                Console.WriteLine("Done. (in {0}ms)", Stopwatch.ElapsedMilliseconds);
            }
#if DEBUG
            Console.ReadLine();
#endif
        }

        private static bool BeginApplication()
        {
            // Let's try to load the script file specified
            Console.WriteLine("Loading generator...");
            string scriptFilepath = string.Format("{0}/Scripts/{1}.tofc", 
                AppDomain.CurrentDomain.BaseDirectory, Arguments.GeneratorScript);
            try
            {
                Generator = new ToffeeGenerator(scriptFilepath, Arguments.Verbose || Arguments.VerboseGenerator);
            }
            catch (ToffeeGeneratorException e)
            {
                Console.WriteLine("An error occurred while creating the generator:");
                Console.WriteLine("ToffeeGeneratorException: {0}", e.Message);
            }

            // We've successfully loaded the script file, let's make the writer!
            Writer = new ToffeeWriter(Generator, Arguments.Verbose || Arguments.VerboseWriter);

            // Now, let's load the toffee file
            Console.WriteLine("Loading input file...");
            if (Path.GetExtension(Arguments.InputFile) != ".tdl")
            {
                Console.WriteLine("Input file must have '.tdl' extension.");
                return false;
            }
            TdlFile file = new TdlFile(Arguments.BaseNamespace, Arguments.Verbose || Arguments.VerboseParser);
            try
            {
                if (!file.ParseFile(Path.GetFullPath(Arguments.InputFile)))
                {
                    Console.WriteLine("Could not find the input file.");
                    return false;
                }
            }
            catch (TdlParserException e)
            {
                Console.WriteLine("An error occurred while parsing.");
                Console.WriteLine(e.Message);
                return false;
            }
            Console.WriteLine("Hash: {0}", file.Hash);

            // Finally, let's output!
            Console.WriteLine("Generating...");
            try
            {
                object gatherer = ToffeeDataGatherers.Instance.GetGatherer(
                    Path.GetFileNameWithoutExtension(Arguments.GeneratorScript), Writer, file);
                if (gatherer == null)
                {
                    Console.WriteLine("An error occurred while generating.");
                    Console.WriteLine("Couldn't find gatherer for specified script.");
                    return false;
                }
                Writer.Gatherer = gatherer;
                if (Arguments.OutputDirectory != "")
                    Writer.Generate(Path.GetFullPath(Arguments.OutputDirectory));
                else
                    Writer.Generate(Arguments.OutputDirectory);
            }
            catch (ToffeeWriterException e)
            {
                Console.WriteLine("An error occurred while generating.");
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }
    }
}
