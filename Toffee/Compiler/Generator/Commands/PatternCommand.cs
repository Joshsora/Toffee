using Toffee.Compiler.Writer;

namespace Toffee.Compiler.Generator.Commands
{
    public class PatternCommand : IGeneratorCommand
    {
        public string Pattern { get; private set; }

        public PatternCommand(string pattern)
        {
            Pattern = pattern;
        }

        public void Execute(ToffeeWriter writer)
        {
            writer.ExecutePattern(Pattern);
        }

        [GeneratorCommandParser("pattern")]
        public static void Parse(ToffeeGenerator generator, string[] args)
        {
            if (args.Length != 1)
                throw new ToffeeGeneratorException("Invalid number of arguments for 'pattern' command.");
            generator.Stack.Push(new ToffeeGeneratorStackFrame(args[0]));
            generator.Log("Enter Pattern Block (Name: {0})", args[0]);
        }

        [GeneratorCommandParser("endpattern")]
        public static void ParseEnd(ToffeeGenerator generator, string[] args)
        {
            if (generator.Stack.Count < 2)
                throw new ToffeeGeneratorException("Encountered 'endpattern' command without being inside a pattern.");
            if (generator.Stack.Peek().Name == "if")
                throw new ToffeeGeneratorException("Encountered 'endpattern' command instead of 'endif'");
            ToffeeGeneratorStackFrame handle = generator.Stack.Pop();
            generator.Log("Exit Pattern Block (Name: {0})", handle.Name);
            generator.AddPattern(handle.Name, handle.Commands);
        }

        [GeneratorCommandParser("dopattern")]
        public static void ParseDo(ToffeeGenerator generator, string[] args)
        {
            if (args.Length != 1)
                throw new ToffeeGeneratorException("Invalid number of arguments for 'dopattern' command.");
            generator.Log("DoPattern({0})", args[0]);
            generator.AddCommand(new PatternCommand(args[0]));
        }
    }
}
