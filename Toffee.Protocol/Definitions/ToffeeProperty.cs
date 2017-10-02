using System;
using System.Reflection;
using Toffee.Core;
using Toffee.Protocol.Packets.Objects;

namespace Toffee.Protocol.Definitions
{
    public class ToffeeProperty : ToffeeField
    {
        public PropertyInfo PropertyInfo { get; private set; }

        public ToffeeProperty(ToffeeObject owner, PropertyInfo propertyInfo, ToffeeModifiers modifiers)
            : base(owner, propertyInfo.Name, modifiers)
        {
            PropertyInfo = propertyInfo;
        }

        public object Get(object instance)
        {
            if (instance.GetType() != Owner.Type)
                throw new ToffeeException("Tried to get property '{0}' on struct '{1}' but the instance was not the correct type.", Identifier, Owner.Name);
            return PropertyInfo.GetValue(instance, null);
        }

        public void Set(object instance, object value)
        {
            if (instance.GetType() != Owner.Type)
                throw new ToffeeException("Tried to set property '{0}' on struct '{1}' but the instance was not the correct type.", Identifier, Owner.Name);
            if (value.GetType() != PropertyInfo.PropertyType)
                throw new ToffeeException("Tried to set property '{0}' on struct '{1}' but the value was not the correct type.", Identifier, Owner.Name);
            PropertyInfo.SetValue(instance, value, null);
        }

        public override FieldUpdate MakeFieldUpdate(params object[] parameters)
        {
            // Do we have more than one parameter?
            if (parameters.Length != 1)
                throw new ToffeeException("Field update {0}::{1} requires 1 parameter, got {3}.",
                    Owner.Name, Identifier, parameters.Length);

            // Is the parameter the correct Type?
            object parameter = parameters[0];
            if (parameter.GetType() != PropertyInfo.GetType())
                throw new ToffeeException("Invalid parameter type for field update {0}::{1}", Owner.Name, Identifier);

            // Write the parameter to a temporary packet
            ToffeePacket packet = new ToffeePacket(Owner.Network);
            packet.Write(parameter);

            // Return the FieldUpdate
            return new FieldUpdate
            {
                FieldId = FieldId,
                Parameters = packet.GetBytes()
            };
        }
    }
}
