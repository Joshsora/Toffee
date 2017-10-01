using System;
using System.Collections.Generic;
using Toffee.Util;

namespace Toffee.Core.Definitions
{
    public class ToffeeObject
    {
        public ToffeeNetwork Network { get; private set; }
        public Type Type { get; private set; }
        public string FullName
        {
            get
            {
                return Type.FullName;
            }
        }
        public string Name
        {
            get
            {
                return Type.Name;
            }
        }
        public uint ObjectId
        {
            get
            {
                return CRC.CalculateCRC32(Type.FullName);
            }
        }

        public bool IsClass
        {
            get
            {
                return (GetType() == typeof(ToffeeClass));
            }
        }

        public ToffeeClass Class
        {
            get
            {
                if (IsClass)
                    return (ToffeeClass)this;
                return null;
            }
        }

        public bool IsStruct
        {
            get
            {
                return (GetType() == typeof(ToffeeStruct));
            }
        }

        public ToffeeStruct Struct
        {
            get
            {
                if (IsStruct)
                    return (ToffeeStruct)this;
                return null;
            }
        }

        public bool IsService
        {
            get
            {
                return (GetType() == typeof(ToffeeService));
            }
        }

        public ToffeeService Service
        {
            get
            {
                if (IsService)
                    return (ToffeeService)this;
                return null;
            }
        }

        private List<ToffeeField> _Fields { get; set; }
        public ToffeeField[] Fields
        {
            get
            {
                return _Fields.ToArray();
            }
        }
        private Dictionary<string, ToffeeField> FieldIdentifierLookup { get; set; }
        private Dictionary<uint, ToffeeField> FieldLookup { get; set; }

        public ToffeeObject(ToffeeNetwork network, Type type)
        {
            Network = network;
            Type = type;

            _Fields = new List<ToffeeField>();
            FieldIdentifierLookup = new Dictionary<string, ToffeeField>();
            FieldLookup = new Dictionary<uint, ToffeeField>();
        }

        public void AddField(ToffeeField field)
        {
            if (_Fields.Contains(field))
                return;
            _Fields.Add(field);
            FieldIdentifierLookup.Add(field.Identifier, field);
            FieldLookup.Add(field.FieldId, field);
        }

        public ToffeeField GetField(string identifier)
        {
            if (FieldIdentifierLookup.ContainsKey(identifier))
                return FieldIdentifierLookup[identifier];
            return null;
        }

        public ToffeeField GetField(uint fieldId)
        {
            if (FieldLookup.ContainsKey(fieldId))
                return FieldLookup[fieldId];
            return null;
        }

        public T GetField<T>(string identifier) where T : ToffeeField
        {
            if (FieldIdentifierLookup.ContainsKey(Name))
                return (T)FieldIdentifierLookup[Name];
            return null;
        }

        public T GetField<T>(uint fieldId) where T : ToffeeField
        {
            if (FieldLookup.ContainsKey(fieldId))
                return (T)FieldLookup[fieldId];
            return null;
        }

        public bool HasField(string identifier)
        {
            return FieldIdentifierLookup.ContainsKey(identifier);
        }

        public bool HasField(uint fieldId)
        {
            return FieldLookup.ContainsKey(fieldId);
        }

        public bool HasField<T>(string identifier)
        {
            if (FieldIdentifierLookup.ContainsKey(identifier))
                return FieldIdentifierLookup[identifier].GetType() == typeof(T);
            return false;
        }

        public bool HasField<T>(uint fieldId)
        {
            if (FieldLookup.ContainsKey(fieldId))
                return FieldLookup[fieldId].GetType() == typeof(T);
            return false;
        }
    }
}
