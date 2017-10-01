using System;
using System.Collections.Generic;
using System.Reflection;
using Fasterflect;

using Toffee.Compiler.Generator;
using Toffee.Core.Parser.Definitions;

namespace Toffee.Compiler.Writer
{
    public class ToffeeDataGatherers
    {
        public static ToffeeDataGatherers _Instance;
        public static ToffeeDataGatherers Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new ToffeeDataGatherers();
                return _Instance;
            }
        }
        private Dictionary<string, Type> ScriptName2Gatherer { get; set; }

        private ToffeeDataGatherers()
        {
            ScriptName2Gatherer = new Dictionary<string, Type>();
            // Get all the currently loaded assemblies
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                // Get the Types in this assembly
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    ToffeeDataGathererAttribute attribute = type.Attribute<ToffeeDataGathererAttribute>();
                    if (attribute == null)
                        continue;
                    if (type.BaseType != typeof(ToffeeDataGatherer))
                    {
                        Console.WriteLine("Found class with ToffeeDataGathererAttribute but did not inherit ToffeeDataGatherer.");
                        continue;
                    }
                    ScriptName2Gatherer.Add(
                        attribute.ScriptName,
                        type
                    );
                }
            }
        }

        public object GetGatherer(string scriptName, ToffeeWriter writer, TdlFile file)
        {
            if (ScriptName2Gatherer.ContainsKey(scriptName))
                return Activator.CreateInstance(ScriptName2Gatherer[scriptName], writer, file);
            return null;
        }
    }
}
