using System.Collections.Generic;
using System.Linq;

namespace Toffee.Core.Parser.Definitions
{
    public class TdlNamespace
    {
        public TdlFile Root
        {
            get
            {
                if (Parent == null)
                    return (TdlFile)this;
                else
                    return Parent.Root;
            }
        }
        public TdlNamespace Parent { get; private set; }
        public string Identifier { get; private set; }
        public string FullName
        {
            get
            {
                if ((Parent != null) && (Parent.Identifier != ""))
                    return Parent.FullName + "." + Identifier;
                else
                    return Identifier;
            }
        }
        private List<TdlNamespace> _Namespaces { get; set; }
        public TdlNamespace[] Namespaces
        {
            get
            {
                return _Namespaces.ToArray();
            }
        }
        private Dictionary<string, TdlNamespace> NamespaceLookup { get; set; }

        private List<TdlStruct> _Structs { get; set; }
        public TdlStruct[] Structs
        {
            get
            {
                return _Structs.ToArray();
            }
        }
        private Dictionary<string, TdlStruct> StructIdentifierLookup { get; set; }
        private Dictionary<uint, TdlStruct> StructLookup { get; set; }

        private List<TdlClass> _Classes { get; set; }
        public TdlClass[] Classes
        {
            get
            {
                return _Classes.ToArray();
            }
        }
        private Dictionary<string, TdlClass> ClassIdentifierLookup { get; set; }
        private Dictionary<uint, TdlClass> ClassLookup { get; set; }

        private List<TdlService> _Services { get; set; }
        public TdlService[] Services
        {
            get
            {
                return _Services.ToArray();
            }
        }
        private Dictionary<string, TdlService> ServiceIdentifierLookup { get; set; }
        private Dictionary<uint, TdlService> ServiceLookup { get; set; }

        public TdlNamespace(string identifier, TdlNamespace parent = null)
        {
            Identifier = identifier;
            Parent = parent;
            _Namespaces = new List<TdlNamespace>();
            NamespaceLookup = new Dictionary<string, TdlNamespace>();

            _Structs = new List<TdlStruct>();
            StructIdentifierLookup = new Dictionary<string, TdlStruct>();
            StructLookup = new Dictionary<uint, TdlStruct>();

            _Classes = new List<TdlClass>();
            ClassIdentifierLookup = new Dictionary<string, TdlClass>();
            ClassLookup = new Dictionary<uint, TdlClass>();

            _Services = new List<TdlService>();
            ServiceIdentifierLookup = new Dictionary<string, TdlService>();
            ServiceLookup = new Dictionary<uint, TdlService>();
        }

        public TdlNamespace AddNamespace(string identifier)
        {
            string[] namespaces = identifier.Split('.');
            TdlNamespace current = this;
            foreach (string namespaceIdentifier in namespaces)
            {
                if (current.NamespaceLookup.ContainsKey(namespaceIdentifier))
                    current = current.NamespaceLookup[namespaceIdentifier];
                else
                {
                    TdlNamespace tdlNamespace = new TdlNamespace(namespaceIdentifier, current);
                    current._Namespaces.Add(tdlNamespace);
                    current.NamespaceLookup.Add(namespaceIdentifier, tdlNamespace);
                    current = tdlNamespace;
                }
            }

            if (current.Identifier == namespaces.Last())
                return current;
            return null;
        }

        public bool HasNamespace(string identifier, int index = 0)
        {
            string[] namespaces = identifier.Split('.');
            TdlNamespace current = this;
            foreach (string namespaceIdentifier in namespaces)
            {
                if (current.NamespaceLookup.ContainsKey(namespaceIdentifier))
                    current = current.NamespaceLookup[namespaceIdentifier];
                else
                    return false;
            }
            return (current.Identifier == namespaces.Last());
        }

        public TdlNamespace GetNamespace(string identifier)
        {
            string[] namespaces = identifier.Split('.');
            TdlNamespace current = this;
            foreach (string namespaceIdentifier in namespaces)
            {
                if (current.NamespaceLookup.ContainsKey(namespaceIdentifier))
                    current = current.NamespaceLookup[namespaceIdentifier];
                else
                    break;
            }
            if (current.Identifier == namespaces.Last())
                return current;
            return null;
        }

        public TdlStruct AddStruct(string identifier)
        {
            if (identifier == "")
                return null;
            TdlStruct tdlStruct = new TdlStruct(identifier, this);
            _Structs.Add(tdlStruct);
            StructIdentifierLookup.Add(identifier, tdlStruct);
            StructLookup.Add(tdlStruct.ObjectId, tdlStruct);
            Root.AddStruct(tdlStruct);
            return tdlStruct;
        }

        public bool HasStruct(string identifier)
        {
            return StructIdentifierLookup.ContainsKey(identifier);
        }

        public bool HasStruct(uint structId)
        {
            return StructLookup.ContainsKey(structId);
        }

        public TdlStruct GetStruct(string identifier)
        {
            if (HasStruct(identifier))
                return StructIdentifierLookup[identifier];
            return null;
        }

        public TdlStruct GetStruct(uint objectId)
        {
            if (HasStruct(objectId))
                return StructLookup[objectId];
            return null;
        }

        public bool IsStructDefined(string identifier)
        {
            if (HasStruct(identifier))
                return true;
            foreach (TdlNamespace tdlNamespace in Namespaces)
            {
                if (tdlNamespace.IsStructDefined(identifier))
                    return true;
            }
            return false;
        }

        public bool IsStructDefined(uint objectId)
        {
            if (HasStruct(objectId))
                return true;
            foreach (TdlNamespace tdlNamespace in Namespaces)
            {
                if (tdlNamespace.IsStructDefined(objectId))
                    return true;
            }
            return false;
        }

        public TdlStruct FindStruct(string identifier)
        {
            if (HasStruct(identifier))
                return GetStruct(identifier);
            foreach (TdlNamespace tdlNamespace in Namespaces)
            {
                TdlStruct possible = tdlNamespace.FindStruct(identifier);
                if (possible != null)
                    return possible;
            }
            return null;
        }

        public TdlStruct FindStruct(uint objectId)
        {
            if (HasStruct(objectId))
                return GetStruct(objectId);
            foreach (TdlNamespace tdlNamespace in Namespaces)
            {
                TdlStruct possible = tdlNamespace.FindStruct(objectId);
                if (possible != null)
                    return possible;
            }
            return null;
        }

        public TdlClass AddClass(string identifier)
        {
            if (identifier == "")
                return null;
            TdlClass tdlClass = new TdlClass(identifier, this);
            _Classes.Add(tdlClass);
            ClassIdentifierLookup.Add(identifier, tdlClass);
            ClassLookup.Add(tdlClass.ObjectId, tdlClass);
            Root.AddClass(tdlClass);
            return tdlClass;
        }

        public bool HasClass(string identifier)
        {
            return ClassIdentifierLookup.ContainsKey(identifier);
        }

        public bool HasClass(uint objectId)
        {
            return ClassLookup.ContainsKey(objectId);
        }

        public TdlClass GetClass(string identifier)
        {
            if (HasClass(identifier))
                return ClassIdentifierLookup[identifier];
            return null;
        }

        public TdlClass GetClass(uint objectId)
        {
            if (HasClass(objectId))
                return ClassLookup[objectId];
            return null;
        }

        public bool IsClassDefined(string identifier)
        {
            if (HasClass(identifier))
                return true;
            foreach (TdlNamespace tdlNamespace in Namespaces)
            {
                if (tdlNamespace.IsClassDefined(identifier))
                    return true;
            }
            return false;
        }

        public bool IsClassDefined(uint objectId)
        {
            if (HasClass(objectId))
                return true;
            foreach (TdlNamespace tdlNamespace in Namespaces)
            {
                if (tdlNamespace.IsClassDefined(objectId))
                    return true;
            }
            return false;
        }

        public TdlClass FindClass(string identifier)
        {
            if (HasClass(identifier))
                return GetClass(identifier);
            foreach (TdlNamespace tdlNamespace in Namespaces)
            {
                TdlClass possible = tdlNamespace.FindClass(identifier);
                if (possible != null)
                    return possible;
            }
            return null;
        }

        public TdlClass FindClass(uint objectId)
        {
            if (HasClass(objectId))
                return GetClass(objectId);
            foreach (TdlNamespace tdlNamespace in Namespaces)
            {
                TdlClass possible = tdlNamespace.FindClass(objectId);
                if (possible != null)
                    return possible;
            }
            return null;
        }

        public TdlService AddService(string identifier)
        {
            if (identifier == "")
                return null;
            TdlService tdlService = new TdlService(identifier, this);
            _Services.Add(tdlService);
            ServiceIdentifierLookup.Add(identifier, tdlService);
            ServiceLookup.Add(tdlService.ObjectId, tdlService);
            Root.AddService(tdlService);
            return tdlService;
        }

        public bool HasService(string identifier)
        {
            return ServiceIdentifierLookup.ContainsKey(identifier);
        }

        public bool HasService(uint objectId)
        {
            return ServiceLookup.ContainsKey(objectId);
        }

        public TdlService GetService(string identifier)
        {
            if (HasService(identifier))
                return ServiceIdentifierLookup[identifier];
            return null;
        }

        public TdlService GetService(uint objectId)
        {
            if (HasService(objectId))
                return ServiceLookup[objectId];
            return null;
        }

        public bool IsServiceDefined(string identifier)
        {
            if (HasService(identifier))
                return true;
            foreach (TdlNamespace tdlNamespace in Namespaces)
            {
                if (tdlNamespace.IsServiceDefined(identifier))
                    return true;
            }
            return false;
        }

        public bool IsServiceDefined(uint objectId)
        {
            if (HasService(objectId))
                return true;
            foreach (TdlNamespace tdlNamespace in Namespaces)
            {
                if (tdlNamespace.IsServiceDefined(objectId))
                    return true;
            }
            return false;
        }

        public TdlService FindService(string identifier)
        {
            if (HasService(identifier))
                return GetService(identifier);
            foreach (TdlNamespace tdlNamespace in Namespaces)
            {
                TdlService possible = tdlNamespace.FindService(identifier);
                if (possible != null)
                    return possible;
            }
            return null;
        }

        public TdlService FindService(uint objectId)
        {
            if (HasService(objectId))
                return GetService(objectId);
            foreach (TdlNamespace tdlNamespace in Namespaces)
            {
                TdlService possible = tdlNamespace.FindService(objectId);
                if (possible != null)
                    return possible;
            }
            return null;
        }

        public bool IsTypeDefined(string identifier)
        {
            if (IsStructDefined(identifier))
                return true;
            else if (IsClassDefined(identifier))
                return true;
            else if (IsServiceDefined(identifier))
                return true;
            return false;
        }
    }
}