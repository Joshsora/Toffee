using System;
using System.Collections.Generic;
using System.Reflection;
using Fasterflect;

namespace Toffee.Compiler.Generator.Commands
{
    public class GeneratorCommandParsers
    {
        public static GeneratorCommandParsers _Instance;
        public static GeneratorCommandParsers Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new GeneratorCommandParsers();
                return _Instance;
            }
        }
        private Dictionary<string, Action<ToffeeGenerator, string[]>> Keyword2Parser { get; set; }

        public Action<ToffeeGenerator, string[]> this[string keyword]
        {
            get
            {
                if (Keyword2Parser.ContainsKey(keyword))
                    return Keyword2Parser[keyword];
                return null;
            }
        }

        private GeneratorCommandParsers()
        {
            Keyword2Parser = new Dictionary<string, Action<ToffeeGenerator, string[]>>();
            // Get all the currently loaded assemblies
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                // Get the Types in this assembly
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    foreach (MethodInfo methodInfo in type.GetMethods())
                    {
                        GeneratorCommandParserAttribute attribute = methodInfo.Attribute<GeneratorCommandParserAttribute>();
                        if (attribute == null)
                            continue;
                        Keyword2Parser.Add(
                            attribute.Keyword,
                            (Action<ToffeeGenerator, string[]>)Delegate.CreateDelegate (
                                typeof(Action<ToffeeGenerator, string[]>), methodInfo
                            )
                        );
                    }
                }
            }
        }
    }
}
