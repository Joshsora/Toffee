using System;
using System.Collections.Generic;

namespace Toffee.Core.Parser.Definitions
{
    public class TdlMethod : TdlField
    {
        private List<TdlParameter> _Parameters { get; set; }
        public TdlParameter[] Parameters
        {
            get
            {
                return _Parameters.ToArray();
            }
        }
        private Dictionary<string, TdlParameter> ParameterLookup { get; set; }

        public uint MinimumSize { get; private set; }

        public TdlMethod(string identifier, TdlObject owner) : base(identifier, owner)
        {
            if (owner.GetType() == typeof(TdlStruct))
                throw new Exception("TdlMethod cannot be owned by TdlStruct.");
            _Parameters = new List<TdlParameter>();
            ParameterLookup = new Dictionary<string, TdlParameter>();
            MinimumSize = 0;
        }

        public void AddParameter(string identifier, TdlType type)
        {
            if (ParameterLookup.ContainsKey(identifier))
                return;
            TdlParameter parameter = new TdlParameter(identifier, type);
            _Parameters.Add(parameter);
            MinimumSize += parameter.Type.MinimumSize;
            ParameterLookup.Add(identifier, parameter);
            Owner.File.AddStringToHash(identifier);

            if (type.Type == ToffeeValueType.Struct)
                Owner.AddRequiredNamespace(type.StructType.Namespace.FullName);
        }

        public TdlParameter GetParameter(string identifier)
        {
            if (ParameterLookup.ContainsKey(identifier))
                return ParameterLookup[identifier];
            return null;
        }

        public bool HasParameter(string identifier)
        {
            return ParameterLookup.ContainsKey(identifier);
        }
    }
}
