using System;
using System.Collections.Generic;
using System.Reflection;

namespace Toffee.Core.Definitions
{
    public class ToffeeClass : ToffeeStruct
    {
        private List<ToffeeMethod> _Methods { get; set; }
        private ToffeeMethod[] Methods
        {
            get
            {
                return _Methods.ToArray();
            }
        }
        private Dictionary<string, ToffeeMethod> MethodIdentifierLookup { get; set; }
        private Dictionary<uint, ToffeeMethod> MethodLookup { get; set; }

        public ToffeeClass(ToffeeNetwork network, Type type) : base(network, type)
        {
            _Methods = new List<ToffeeMethod>();
            MethodIdentifierLookup = new Dictionary<string, ToffeeMethod>();
            MethodLookup = new Dictionary<uint, ToffeeMethod>();
        }

        public void AddMethod(MethodInfo methodInfo, ToffeeModifiers modifiers = ToffeeModifiers.None)
        {
            ToffeeMethod method = new ToffeeMethod(this, methodInfo, modifiers);
            if (HasField(method.Identifier))
                throw new ToffeeException("Class '{0}' already has field with name: '{1}'.", Name, method.Identifier);
            if (HasField(method.FieldId))
                throw new ToffeeException("Class '{0}' already has field with id: '{1}'.", Name, method.FieldId);

            AddField(method);
            _Methods.Add(method);
            MethodIdentifierLookup.Add(method.Identifier, method);
            MethodLookup.Add(method.FieldId, method);
        }

        public ToffeeMethod GetMethod(string identifier)
        {
            return GetField<ToffeeMethod>(identifier);
        }

        public ToffeeMethod GetMethod(uint fieldId)
        {
            return GetField<ToffeeMethod>(fieldId);
        }

        public void InvokeMethod(object instance, string identifier, params object[] parameters)
        {
            if (!HasField<ToffeeMethod>(identifier))
                throw new ToffeeException("Tried to invoke method '{0}' on class '{1}' but it doesn't exist.", Name, identifier);
            ToffeeMethod method = GetField<ToffeeMethod>(identifier);
            method.Invoke(instance, parameters);
        }

        public void InvokeMethod(object instance, uint fieldId, params object[] parameters)
        {
            if (!HasField<ToffeeMethod>(fieldId))
                throw new ToffeeException("Tried to invoke method '{0}' on class '{1}' but it doesn't exist.", Name, fieldId);
            ToffeeMethod method = GetField<ToffeeMethod>(fieldId);
            method.Invoke(instance, parameters);
        }
    }
}
