using System.Collections.Generic;
using Toffee.Util;

namespace Toffee.Core.Parser.Definitions
{
    public class TdlObject
    {
        public TdlFile File
        {
            get
            {
                return Namespace.Root;
            }
        }
        public TdlNamespace Namespace { get; private set; }

        public string Identifier { get; private set; }
        public string FullName
        {
            get
            {
                return Namespace.FullName + "." + Identifier;
            }
        }
        public uint ObjectId { get; private set; }

        private List<string> _RequiredNamespaces;
        public string[] RequiredNamespaces
        {
            get
            {
                return _RequiredNamespaces.ToArray();
            }
        }

        private List<TdlField> _Fields;
        public TdlField[] Fields
        {
            get
            {
                return _Fields.ToArray();
            }
        }

        public TdlObject(string identifier, TdlNamespace parent)
        {
            Namespace = parent;
            Identifier = identifier;
            ObjectId = CRC.CalculateCRC32(FullName);
            _RequiredNamespaces = new List<string>();
            _Fields = new List<TdlField>();
            File.AddStringToHash(FullName);
        }

        public void AddRequiredNamespace(string namespaceName)
        {
            if ((!_RequiredNamespaces.Contains(namespaceName)) &&
                (namespaceName != Namespace.FullName) &&
                (namespaceName != ""))
                _RequiredNamespaces.Add(namespaceName);
        }

        protected void AddField(TdlField field)
        {
            if (!_Fields.Contains(field))
                _Fields.Add(field);
        }

        public TdlField[] FindFieldsWithModifier(ToffeeModifiers modifier)
        {
            return FindFieldsWithModifier<TdlField>(modifier);
        }

        public T[] FindFieldsWithModifier<T>(ToffeeModifiers modifier) where T : TdlField
        {
            List<T> fields = new List<T>();
            foreach (TdlField field in _Fields)
            {
                if ((field.HasModifier(modifier)) && (field is T))
                    fields.Add((T)field);
            }
            return fields.ToArray();
        }
    }
}
