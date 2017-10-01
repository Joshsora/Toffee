using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Fasterflect;

using Toffee.Core;
using Toffee.Logging;

namespace Toffee.Server.Roles
{
    public class ToffeeRole
    {
        private static Dictionary<string, Type> RoleLookup { get; set; }
        public static Type[] RoleTypes
        {
            get
            {
                return RoleLookup.Values.ToArray();
            }
        }

        public static void BuildRoleLookup(LoggerCategory log = null)
        {
            if (RoleLookup != null)
                return;
            if (log != null)
                log.Debug("Building role lookup...");
#if DEBUG
            else
                Console.WriteLine("Building role lookup...");
#endif
            RoleLookup = new Dictionary<string, Type>();
            
            // Get all the currently loaded assemblies
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                // Get the Types in this assembly
                Type[] types = assembly.Types().ToArray();
                foreach (Type type in types)
                {
                    // Is this Type a ToffeeRole?
                    if (!type.HasAttribute<ToffeeRoleAttribute>())
                        continue;

                    // Does it implement IRole?
                    if (!type.Implements<IRole>())
                        throw new ToffeeException("Found proposed role '{0}', but it does not implement IRole.", type.Name);

                    // Get the attribute
                    ToffeeRoleAttribute attribute = type.Attribute<ToffeeRoleAttribute>();
                    RoleLookup.Add(attribute.Name, type);
                    if (log != null)
                        log.Debug("Found role: {0}", attribute.Name);
#if DEBUG
                    else
                        Console.WriteLine("Found role: {0}", attribute.Name);
#endif
                }
            }
            if (log != null)
                log.Debug("Found {0} role(s).", RoleTypes.Length);
#if DEBUG
            else
                Console.WriteLine("Found {0} role(s).", RoleTypes.Length);
#endif
        }

        public static bool HasRole(string name)
        {
            return RoleLookup.ContainsKey(name);
        }

        public static Type GetRole(string name)
        {
            if (HasRole(name))
                return RoleLookup[name];
            return null;
        }
    }
}
