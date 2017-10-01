using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fasterflect;

using Toffee.Core.Definitions.Attributes;
using Toffee.Core.Parser.Definitions;

namespace Toffee.Core.Definitions
{
    public class ToffeeNetwork
    {
        public const string StandardToffeeNetwork = "StdToffee";
        private static Dictionary<Tuple<string, string>, ToffeeNetwork> NetworkCache { get; set; }
        private static List<Type> TypeCache { get; set; }

        public string Name { get; private set; }
        public string Suffix { get; private set; }
        private uint _Hash { get; set; }
        public uint Hash { get; private set; }
        private List<string> Networks { get; set; }

        private Dictionary<Type, List<ToffeeObject>> Objects { get; set; }
        private Dictionary<Type, Dictionary<string, ToffeeObject>> ObjectIdentifierLookup { get; set; }
        private Dictionary<Type, Dictionary<uint, ToffeeObject>> ObjectLookup { get; set; }

        public ToffeeStruct[] Structs
        {
            get
            {
                return GetObjects<ToffeeStruct>();
            }
        }

        public ToffeeClass[] Classes
        {
            get
            {
                return GetObjects<ToffeeClass>();
            }
        }

        public ToffeeService[] Services
        {
            get
            {
                return GetObjects<ToffeeService>();
            }
        }

        static ToffeeNetwork()
        {
            NetworkCache = new Dictionary<Tuple<string, string>, ToffeeNetwork>();
        }

        private ToffeeNetwork(string name, string suffix="")
        {
            Name = name;
            Suffix = suffix;
            Hash = 0;
            AddStringToHash(Name);

            Objects = new Dictionary<Type, List<ToffeeObject>>();
            ObjectIdentifierLookup = new Dictionary<Type, Dictionary<string, ToffeeObject>>();
            ObjectLookup = new Dictionary<Type, Dictionary<uint, ToffeeObject>>();
            Networks = new List<string>();

            AddObjectsToNetwork(Name);
        }

        public void AddObjectsToNetwork(string networkName)
        {
            if (Networks.Contains(networkName))
                return;
            Networks.Add(networkName);
            bool addToHash = networkName == Name;
            
            // Do we not have a Type cache?
            if (TypeCache == null)
            {
                // Create the type cache, and populate it this time around
                TypeCache = new List<Type>();

                // Get all the currently loaded assemblies
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly assembly in assemblies)
                {
                    // Get the Types in this assembly
                    Type[] types = assembly.Types().ToArray();
                    foreach (Type type in types)
                    {
                        // If this type a ToffeeStructure?
                        if (type.HasAttribute<ToffeeStructureAttribute>())
                        {
                            ToffeeStructureAttribute attribute = type.Attribute<ToffeeStructureAttribute>();
                            if (attribute.NetworkName == networkName)
                                AddStructure(type, addToHash);
                            TypeCache.Add(type);
                        }

                        // If this type a ToffeeClass?
                        if (type.HasAttribute<ToffeeClassAttribute>())
                        {
                            ToffeeClassAttribute attribute = type.Attribute<ToffeeClassAttribute>();
                            if (attribute.NetworkName == networkName)
                                AddClass(type, addToHash);
                            TypeCache.Add(type);
                        }
                        // If this type a ToffeeService?
                        else if (type.HasAttribute<ToffeeServiceAttribute>())
                        {
                            ToffeeServiceAttribute attribute = type.Attribute<ToffeeServiceAttribute>();
                            if (attribute.NetworkName == networkName)
                                AddService(type, addToHash);
                            TypeCache.Add(type);
                        }
                    }
                }
            }
            else
            {
                // We already have a cache of types that are usable to Toffee
                foreach (Type type in TypeCache)
                {
                    // If this type a ToffeeStructure?
                    if (type.HasAttribute<ToffeeStructureAttribute>())
                    {
                        ToffeeStructureAttribute attribute = type.Attribute<ToffeeStructureAttribute>();
                        if (attribute.NetworkName == networkName)
                            AddStructure(type, addToHash);
                    }

                    // If this type a ToffeeClass?
                    if (type.HasAttribute<ToffeeClassAttribute>())
                    {
                        ToffeeClassAttribute attribute = type.Attribute<ToffeeClassAttribute>();
                        if (attribute.NetworkName == networkName)
                            AddClass(type, addToHash);
                    }
                    // If this type a ToffeeService?
                    else if (type.HasAttribute<ToffeeServiceAttribute>())
                    {
                        ToffeeServiceAttribute attribute = type.Attribute<ToffeeServiceAttribute>();
                        if (attribute.NetworkName == networkName)
                            AddService(type, addToHash);
                    }
                }
            }
        }

        private void AddStructure(Type type, bool addToHash)
        {
            ToffeeStruct structure = new ToffeeStruct(this, type);
            if (addToHash)
                AddStringToHash(type.FullName);

            Dictionary<int, PropertyInfo> ordered = new Dictionary<int, PropertyInfo>();

            // Go through each property on this structure
            foreach (PropertyInfo propertyInfo in type.Properties())
            {
                // Is this part of the struct sent over Toffee?
                if (!propertyInfo.HasAttribute<ToffeeStructurePropertyAttribute>())
                    continue;

                // Get the Attribute
                ToffeeStructurePropertyAttribute attribute = 
                    propertyInfo.Attribute<ToffeeStructurePropertyAttribute>();

                // Add the Type to the order
                ordered.Add(attribute.Position, propertyInfo);

                if (addToHash)
                    // Add this property to the hash
                    AddStringToHash(propertyInfo.Name);
            }

            // Sort the order
            List<int> orderedKeys = ordered.Keys.ToList();
            orderedKeys.Sort();
            foreach (int key in orderedKeys)
                structure.AddProperty(ordered[key]);

            AddObject(structure);
        }

        private void AddClass(Type type, bool addToHash)
        {
            ToffeeClass tClass = new ToffeeClass(this, type);
            if (addToHash)
                AddStringToHash(type.FullName);

            // Go through each property on this class
            foreach (PropertyInfo propertyInfo in type.Properties())
            {
                // Can this be sent as a field?
                if (!propertyInfo.HasAttribute<ToffeePropertyAttribute>())
                    continue;

                // Get the Attribute
                ToffeePropertyAttribute attribute =
                    propertyInfo.Attribute<ToffeePropertyAttribute>();

                if (addToHash)
                    // Add this property to the hash
                    AddStringToHash(propertyInfo.Name);

                // Add the property
                tClass.AddProperty(propertyInfo, attribute.Modifiers);
            }

            // Go through each method on this class
            foreach (MethodInfo methodInfo in type.Methods())
            {
                // Can this be sent as a field?
                if (!methodInfo.HasAttribute<ToffeeMethodAttribute>())
                    continue;

                // Get the Attribute
                ToffeeMethodAttribute attribute =
                    methodInfo.Attribute<ToffeeMethodAttribute>();

                if (addToHash)
                {
                    // Add this method to the hash
                    AddStringToHash(methodInfo.Name);
                    foreach (ParameterInfo parameter in methodInfo.Parameters())
                        AddStringToHash(parameter.Name);
                }

                // Add the method
                tClass.AddMethod(methodInfo, attribute.Modifiers);
            }

            AddObject(tClass);
        }

        private void AddService(Type type, bool addToHash)
        {
            ToffeeService service = new ToffeeService(this, type);
            if (addToHash)
                AddStringToHash(type.FullName);

            // Go through each method on this class
            foreach (MethodInfo methodInfo in type.Methods())
            {
                // Can this be sent as a field?
                if (!methodInfo.HasAttribute<ToffeeMethodAttribute>())
                    continue;

                // Get the Attribute
                ToffeeMethodAttribute attribute =
                    methodInfo.Attribute<ToffeeMethodAttribute>();

                if (addToHash)
                {
                    // Add this method to the hash
                    AddStringToHash(methodInfo.Name);
                    foreach (ParameterInfo parameterInfo in methodInfo.Parameters())
                        AddStringToHash(parameterInfo.Name);
                }

                // Add the method
                service.AddMethod(methodInfo, attribute.Modifiers);
            }

            AddObject(service);
        }

        public bool ContainsNetwork(string networkName)
        {
            return Networks.Contains(networkName);
        }

        private void AddObject(ToffeeObject tObject)
        {
            if (tObject.Network != this)
                throw new ToffeeException("Tried to add object to network '{0}' but object belongs to '{1}'", Name, tObject.Network.Name);
            if (Objects.ContainsKey(tObject.GetType()))
            {
                Objects[tObject.GetType()].Add(tObject);
                ObjectIdentifierLookup[tObject.GetType()].Add(tObject.FullName, tObject);
                ObjectLookup[tObject.GetType()].Add(tObject.ObjectId, tObject);
            }
            else
            {
                Objects.Add(tObject.GetType(), new List<ToffeeObject>());
                ObjectIdentifierLookup.Add(tObject.GetType(), new Dictionary<string, ToffeeObject>());
                ObjectLookup.Add(tObject.GetType(), new Dictionary<uint, ToffeeObject>());
                Objects[tObject.GetType()].Add(tObject);
                ObjectIdentifierLookup[tObject.GetType()].Add(tObject.FullName, tObject);
                ObjectLookup[tObject.GetType()].Add(tObject.ObjectId, tObject);
            }
        }

        public static ToffeeNetwork CreateNetwork(string networkName, string suffix = "", bool includeStd = true)
        {
            if (NetworkCache.ContainsKey(new Tuple<string, string>(networkName, suffix)))
            {
                ToffeeNetwork cachedNetwork = NetworkCache[new Tuple<string, string>(networkName, suffix)];
                if ((includeStd) && (cachedNetwork.ContainsNetwork(StandardToffeeNetwork)))
                    return cachedNetwork;
                if ((!includeStd) && (!cachedNetwork.ContainsNetwork(StandardToffeeNetwork)))
                    return cachedNetwork;
            }
            
            ToffeeNetwork network = new ToffeeNetwork(networkName);
            if (includeStd)
                network.AddObjectsToNetwork(StandardToffeeNetwork);
            NetworkCache.Add(new Tuple<string, string>(networkName, suffix), network);
            return network;
        }

        public bool HasObject(string fullName)
        {
            foreach (Type type in ObjectIdentifierLookup.Keys)
                if (ObjectIdentifierLookup[type].ContainsKey(fullName))
                    return true;
            return false;
        }

        public bool HasObject(uint objectId)
        {
            foreach (Type type in ObjectLookup.Keys)
                if (ObjectLookup[type].ContainsKey(objectId))
                    return true;
            return false;
        }

        public ToffeeObject GetObject(string fullName)
        {
            foreach (Type type in ObjectLookup.Keys)
                if (ObjectIdentifierLookup[type].ContainsKey(fullName))
                    return ObjectIdentifierLookup[type][fullName];
            return null;
        }

        public ToffeeObject GetObject(uint objectId)
        {
            foreach (Type type in ObjectLookup.Keys)
                if (ObjectLookup[type].ContainsKey(objectId))
                    return ObjectLookup[type][objectId];
            return null;
        }

        public bool HasObject<T>(string fullName)
        {
            if (ObjectIdentifierLookup.ContainsKey(typeof(T)))
                return ObjectIdentifierLookup[typeof(T)].ContainsKey(fullName);
            return false;
        }

        public bool HasObject<T>(uint objectId)
        {
            if (ObjectLookup.ContainsKey(typeof(T)))
                return ObjectLookup[typeof(T)].ContainsKey(objectId);
            return false;
        }

        public T[] GetObjects<T>() where T : ToffeeObject
        {
            if (Objects.ContainsKey(typeof(T)))
                return (T[])Objects[typeof(T)].ToArray();
            return new T[0];
        }

        public T GetObject<T>(string fullName) where T : ToffeeObject
        {
            if (ObjectIdentifierLookup.ContainsKey(typeof(T)))
            {
                if (ObjectIdentifierLookup[typeof(T)].ContainsKey(fullName))
                    return (T)ObjectIdentifierLookup[typeof(T)][fullName];
            }
            return null;
        }

        public T GetObject<T>(uint objectId) where T : ToffeeObject
        {
            if (ObjectLookup.ContainsKey(typeof(T)))
            {
                if (ObjectLookup[typeof(T)].ContainsKey(objectId))
                    return (T)ObjectLookup[typeof(T)][objectId];
            }
            return null;
        }

        private void AddStringToHash(string str)
        {
            if (str == null)
                return;
            Hash += (uint)str.Length;
            foreach (char c in str)
                Hash += (byte)c;
        }
    }
}
