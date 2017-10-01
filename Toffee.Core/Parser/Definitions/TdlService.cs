using System.Collections.Generic;

namespace Toffee.Core.Parser.Definitions
{
    public class TdlService : TdlObject
    {
        private List<TdlMethod> _Methods { get; set; }
        public TdlMethod[] Methods
        {
            get
            {
                return _Methods.ToArray();
            }
        }
        private Dictionary<uint, TdlMethod> MethodLookup { get; set; }
        private Dictionary<string, TdlMethod> MethodIdentifierLookup { get; set; }

        public TdlService(string identifier, TdlNamespace parent) : base(identifier, parent)
        {
            _Methods = new List<TdlMethod>();
            MethodLookup = new Dictionary<uint, TdlMethod>();
            MethodIdentifierLookup = new Dictionary<string, TdlMethod>();
        }

        public TdlMethod AddMethod(string identifier)
        {
            if (MethodIdentifierLookup.ContainsKey(identifier))
                return null;
            TdlMethod method = new TdlMethod(identifier, this);
            _Methods.Add(method);
            MethodLookup.Add(method.FieldId, method);
            MethodIdentifierLookup.Add(identifier, method);
            AddField(method);
            return method;
        }

        public TdlMethod GetMethod(string identifier)
        {
            if (MethodIdentifierLookup.ContainsKey(identifier))
                return MethodIdentifierLookup[identifier];
            return null;
        }

        public bool HasMethod(string identifier)
        {
            return MethodIdentifierLookup.ContainsKey(identifier);
        }
    }
}
