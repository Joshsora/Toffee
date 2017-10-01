using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Toffee.Compiler.Generator;

namespace Toffee.Compiler.Writer
{
    public class ToffeeWriter
    {
        public bool Verbose { get; set; }
        public object Gatherer { get; set; }
        public ToffeeGenerator Generator { get; private set; }
        public List<string> Defines { get; private set; }
        private Stack<ToffeeWriterStackFrame> Stack { get; set; }
        public Stream Stream
        {
            get
            {
                if (Stack.Count > 0)
                    return Stack.Peek().Stream;
                return null;
            }

            set
            {
                if (Stack.Count > 0)
                    Stack.Peek().Stream = value;
            }
        }
        public StreamWriter Writer
        {
            get
            {
                if (Stack.Count > 0)
                    return Stack.Peek().Writer;
                return null;
            }
        }

        public Dictionary<string, string> Data { get; private set; }
        public event Action<string> OnGetData;

        public string OutputDirectory { get; private set; }
        public bool Indent { get; set; }
        private int _IndentLevel;
        public int IndentLevel
        {
            get
            {
                return _IndentLevel;
            }

            set
            {
                if (value < 0)
                    return;
                _IndentLevel = value;
            }
        }
        public bool AppendNewline { get; set; }

        public ToffeeWriter(ToffeeGenerator generator, bool verbose = false)
        {
            Generator = generator;
            Verbose = verbose;
            OnGetData = delegate { };
        }

        public void Log(string message)
        {
            if (Verbose)
                Console.WriteLine("Writer: " + message);
        }

        public void Log(string message, params object[] args)
        {
            if (Verbose)
                Console.WriteLine("Writer: " + message, args);
        }

        public void ExecutePattern(string pattern)
        {
            if (!Generator.HasPattern(pattern))
                return;

            ToffeeWriterStackFrame stackFrame =
                new ToffeeWriterStackFrame(pattern, Generator.GetPatternCode(pattern));
            stackFrame.Stream = Stream;
            stackFrame.Writer = Writer;
            Stack.Push(stackFrame);
            stackFrame.Execute(this);
            Stack.Pop();
        }

        public void Repeat()
        {
            if (Stack.Count > 0)
            {
                Log("Repeating current pattern: '{0}'", Stack.Peek().Name);
                Stack.Peek().Execute(this);
            }
        }

        public void Define(string define)
        {
            if (!Defines.Contains(define))
                Defines.Add(define);
            Log("Defined '{0}'", define);
        }

        public void Undefine(string define)
        {
            if (Defines.Contains(define))
                Defines.Remove(define);
            Log("Undefined '{0}'", define);
        }

        public void GetData(string requested)
        {
            if (Gatherer == null)
                OnGetData(requested);
            else
            {
                PropertyInfo[] properties = Gatherer.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    if (property.Name == requested)
                    {
                        MethodInfo get = property.GetGetMethod();
                        get.Invoke(Gatherer, new object[] { });
                    }
                }
            }
        }

        public void AddData(string name, string value)
        {
            if (Data.ContainsKey(name))
                Data[name] = value;
            else
                Data.Add(name, value);
            Log("Data '{0}' = '{1}' added", name, value);
        }

        public void RemoveData(string name)
        {
            if (Data.ContainsKey(name))
                Data.Remove(name);
            Log("Data '{0}' removed", name);
        }

        public string GetLineToWrite(string original)
        {
            string value = "";
            if (Indent)
            {
                for (int i = 0; i < IndentLevel; i++)
                    value += "\t";
            }
            value += original;

            // Replace '$DATA' with the values we have
            foreach (KeyValuePair<string, string> keyval in Data)
                value = value.Replace("$" + keyval.Key, keyval.Value);

            // Replace '$PATTERN' with the data that a pattern would return
            foreach (string pattern in Generator.GetPatterns())
            {
                if (value.Contains("$" + pattern))
                {
                    ToffeeWriterStackFrame stackFrame =
                        new ToffeeWriterStackFrame(pattern, Generator.GetPatternCode(pattern));
                    stackFrame.Stream = new MemoryStream();
                    Stack.Push(stackFrame);
                    stackFrame.Execute(this);
                    Writer.Flush();
                    Stack.Pop();

                    stackFrame.Stream.Seek(0, SeekOrigin.Begin);
                    StreamReader reader = new StreamReader(stackFrame.Stream);
                    string returnVal = reader.ReadToEnd();
                    reader.Dispose();
                    value = value.Replace("$" + pattern, returnVal);
                }
            }
            return value;
        }

        public void Generate(string outputDirectory)
        {
            OutputDirectory = outputDirectory;
            Indent = true;
            IndentLevel = 0;
            AppendNewline = true;
            Defines = new List<string>();
            Data = new Dictionary<string, string>();
            Stack = new Stack<ToffeeWriterStackFrame>();

            ToffeeWriterStackFrame stackFrame =
                new ToffeeWriterStackFrame("MAIN", Generator.GetGeneratorCode());
            Stack.Push(stackFrame);
            stackFrame.Execute(this);
            Stack.Pop();
            Log("Done!");
        }
    }
}
