using System.Collections.Generic;

namespace Toffee.Core.Parser.Definitions
{
    public class TdlStruct : TdlObject
    {
        private List<TdlProperty> _Properties;
        public TdlProperty[] Properties
        {
            get
            {
                return _Properties.ToArray();
            }
        }
        private Dictionary<uint, TdlProperty> PropertyLookup { get; set; }
        private Dictionary<string, TdlProperty> PropertyIdentifierLookup { get; set; }

        public uint MinimumSize { get; private set; }

        public TdlStruct(string identifier, TdlNamespace parent) : base (identifier, parent)
        {
            _Properties = new List<TdlProperty>();
            PropertyLookup = new Dictionary<uint, TdlProperty>();
            PropertyIdentifierLookup = new Dictionary<string, TdlProperty>();            
        }

        public void AddProperty(string identifier, TdlType type, ToffeeModifiers modifiers = ToffeeModifiers.None)
        {
            if (PropertyIdentifierLookup.ContainsKey(identifier))
                return;

            TdlProperty property = new TdlProperty(identifier, this, type, modifiers);
            _Properties.Add(property);
            PropertyLookup.Add(property.FieldId, property);
            PropertyIdentifierLookup.Add(identifier, property);
            AddField(property);
            MinimumSize += type.MinimumSize;

            if (type.Type == ToffeeValueType.Struct)
                AddRequiredNamespace(type.StructType.Namespace.FullName);
        }

        public bool HasProperty(string identifier)
        {
            return (PropertyIdentifierLookup.ContainsKey(identifier));
        }

        public bool HasProperty(uint fieldId)
        {
            return (PropertyLookup.ContainsKey(fieldId));
        }
    }
}
