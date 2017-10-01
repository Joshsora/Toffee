using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Toffee.Compiler.Generator.Commands;

namespace Toffee.Compiler.Generator
{
    public class ToffeeGenerator
    {
        public bool Verbose { get; set; }
        public Stack<ToffeeGeneratorStackFrame> Stack { get; private set; }
        private Dictionary<string, List<IGeneratorCommand>> Patterns { get; set; }
        private List<IGeneratorCommand> Commands { get; set; }
        private int ScriptLinesPosition { get; set; }
        private string[] ScriptLines { get; set; }
        public bool CanRead
        {
            get
            {
                return (ScriptLinesPosition < ScriptLines.Length);
            }
        }
        public string Line
        {
            get
            {
                return ScriptLines[ScriptLinesPosition++];
            }
        }

        public ToffeeGenerator(string generatorScriptPath, bool verbose = false)
        {
            Verbose = verbose;
            if (File.Exists(generatorScriptPath))
                ReadScript(generatorScriptPath);
            else
                throw new ToffeeGeneratorException("Could not find generator script!");
        }

        private void ReadScript(string scriptPath)
        {
            Patterns = new Dictionary<string, List<IGeneratorCommand>>();
            Commands = new List<IGeneratorCommand>();
            Stack = new Stack<ToffeeGeneratorStackFrame>();
            Stack.Push(new ToffeeGeneratorStackFrame("MAIN"));

            ScriptLines = File.ReadAllLines(scriptPath);
            ScriptLinesPosition = 0;
            while (CanRead)
            {
                string line = Line.TrimStart();
                if (line.Length > 0)
                {
                    if (line[0] == '#')
                    {
                        string keyword;
                        string[] arguments = line.Split(' ');
                        if (arguments.Length > 1)
                        {
                            keyword = arguments[0].Remove(0, 1);
                            arguments = arguments.Skip(1).ToArray();
                        }
                        else
                        {
                            keyword = line.Remove(0, 1);
                            arguments = new string[] { };
                        }
                        if (keyword == "")
                            continue;
                        try
                        {
                            GeneratorCommandParsers.Instance[keyword](this, arguments);
                        }
                        catch (NullReferenceException)
                        {
                            throw new ToffeeGeneratorException(
                                "(line: {0}) Encountered unknown command: '{1}'", ScriptLinesPosition, keyword);
                        }
                    }
                    else
                        AddCommand(new WriteCommand(this, line));
                }
                else
                    AddCommand(new WriteCommand(this, ""));
            }
            if (Stack.Count != 1)
                throw new ToffeeGeneratorException("Missing 'end' for {0}", Stack.Peek().Name);
            Commands = Stack.Pop().Commands;
        }

        public void Log(string message)
        {
            if (Verbose)
                Console.WriteLine("Generator: " + message);
        }

        public void Log(string message, params object[] args)
        {
            if (Verbose)
                Console.WriteLine("Generator: " + message, args);
        }

        public void AddCommand(IGeneratorCommand command)
        {
            if (Stack.Count > 0)
                Stack.Peek().Commands.Add(command);
        }

        public IGeneratorCommand[] GetGeneratorCode()
        {
            return Commands.ToArray();
        }

        public void AddPattern(string name, List<IGeneratorCommand> commands)
        {
            if (Patterns.ContainsKey(name))
                throw new ToffeeGeneratorException("Cannot have two patterns that share the same name. Pattern '{0}' already exists.", name);
            Patterns.Add(name, commands);
            Log("Add Pattern (Name: {0}, Size: {1})", name, commands.Count);
        }

        public bool HasPattern(string name)
        {
            return Patterns.ContainsKey(name);
        }

        public string[] GetPatterns()
        {
            return Patterns.Keys.ToArray();
        }

        public IGeneratorCommand[] GetPatternCode(string name)
        {
            if (HasPattern(name))
                return Patterns[name].ToArray();
            return null;
        }
    }
}
