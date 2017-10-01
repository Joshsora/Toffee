using System;
using System.Collections.Generic;
using System.Reflection;
using Fasterflect;

namespace Toffee.Core.Definitions
{
    public class ToffeeStruct : ToffeeObject
    {
        private List<ToffeeProperty> _Properties { get; set; }
        public ToffeeProperty[] Properties
        {
            get
            {
                return _Properties.ToArray();
            }
        }
        private Dictionary<string, ToffeeProperty> PropertyIdentifierLookup { get; set; }
        private Dictionary<uint, ToffeeProperty> PropertyLookup { get; set; }

        public bool ImplementsInterface
        {
            get
            {
                return Type.Implements<IToffeeStructure>();
            }
        }

        public int MinimumSize { get; private set; }        

        public ToffeeStruct(ToffeeNetwork network, Type type) : base(network, type)
        {
            _Properties = new List<ToffeeProperty>();
            PropertyIdentifierLookup = new Dictionary<string, ToffeeProperty>();
            PropertyLookup = new Dictionary<uint, ToffeeProperty>();
            MinimumSize = 0;
        }

        public object CreateInstance()
        {
            return Activator.CreateInstance(Type);
        }

        public void AddProperty(PropertyInfo propertyInfo, ToffeeModifiers modifiers = ToffeeModifiers.None)
        {
            ToffeeProperty property = new ToffeeProperty(this, propertyInfo, modifiers);
            if (HasField(property.Identifier))
                throw new ToffeeException("Struct '{0}' already has field with name: '{1}'.", Name, property.Identifier);
            if (HasField(property.FieldId))
                throw new ToffeeException("Struct '{0}' already has field with id: '{1}'.", Name, property.FieldId);

            TypeCode type = Type.GetTypeCode(propertyInfo.PropertyType);
            switch (type)
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Char:
                    MinimumSize++;
                    break;
                case TypeCode.UInt16:
                case TypeCode.Int16:
                    MinimumSize += 2;
                    break;
                case TypeCode.String:
                case TypeCode.UInt32:
                case TypeCode.Int32:
                case TypeCode.Single:
                    MinimumSize += 4;
                    break;
                case TypeCode.UInt64:
                case TypeCode.Int64:
                case TypeCode.Double:
                    MinimumSize += 8;
                    break;
                case TypeCode.Object:
                    if (propertyInfo.PropertyType.IsArray)
                        MinimumSize += 4;
                    break;
            }

            AddField(property);
            _Properties.Add(property);
            PropertyIdentifierLookup.Add(property.Identifier, property);
            PropertyLookup.Add(property.FieldId, property);
        }

        public ToffeeProperty GetProperty(string identifier)
        {
            return GetField<ToffeeProperty>(identifier);
        }

        public ToffeeProperty GetProperty(uint fieldId)
        {
            return GetField<ToffeeProperty>(fieldId);
        }

        public void SetProperty(object instance, string identifier, object value)
        {
            if (!HasField<ToffeeProperty>(identifier))
                throw new ToffeeException("Tried to set property '{0}' on struct '{1}' but it doesn't exist.", identifier, Name);
            ToffeeProperty property = GetField<ToffeeProperty>(identifier);
            if (instance.GetType() != Type)
                throw new ToffeeException("Tried to set property '{0}' on struct '{1}' but instance was not the correct type.", identifier, Name);
            if (value.GetType() != property.PropertyInfo.PropertyType)
                throw new ToffeeException("Tried to set property '{0}' on struct '{1}' but value was not the correct type.", identifier, Name);
            property.PropertyInfo.SetValue(instance, value, null);
        }

        public object GetProperty(object instance, string identifier)
        {
            if (!HasField<ToffeeProperty>(identifier))
                throw new ToffeeException("Tried to get property '{0}' on struct '{1}' but it doesn't exist.", identifier, Name);
            ToffeeProperty property = GetField<ToffeeProperty>(identifier);
            if (instance.GetType() != Type)
                throw new ToffeeException("Tried to get property '{0}' on struct '{1}' but instance was not the correct type.", identifier, Name);
            return property.PropertyInfo.GetValue(instance, null);
        }

        public void SetProperty(object instance, uint fieldId, object value)
        {
            if (!HasField<ToffeeProperty>(fieldId))
                throw new ToffeeException("Tried to set property with fieldId '{0}' on struct '{1}' but it doesn't exist.", fieldId, Name);
            ToffeeProperty property = GetField<ToffeeProperty>(fieldId);
            if (instance.GetType() != property.Owner.Type)
                throw new ToffeeException("Tried to set property with fieldId '{0}' on struct '{1}' but instance was not the correct type.", fieldId, Name);
            if (value.GetType() != property.PropertyInfo.PropertyType)
                throw new ToffeeException("Tried to set property with fieldId '{0}' on struct '{1}' but value was not the correct type.", fieldId, Name);
            property.PropertyInfo.SetValue(instance, value, null);
        }

        public object GetProperty(object instance, uint fieldId)
        {
            if (!HasField<ToffeeProperty>(fieldId))
                throw new ToffeeException("Tried to get property with fieldId '{0}' on struct '{1}' but it doesn't exist.", fieldId, Name);
            ToffeeProperty property = GetField<ToffeeProperty>(fieldId);
            if (instance.GetType() != Type)
                throw new ToffeeException("Tried to get property with fieldId '{0}' on struct '{1}' but instance was not the correct type.", fieldId, Name);
            return property.PropertyInfo.GetValue(instance, null);
        }
    }
}
