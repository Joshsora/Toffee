using System;
using System.Linq;
using System.Reflection;

using Toffee.Core;
using Toffee.Protocol.Packets;

namespace Toffee.Protocol.Definitions
{
    public class ToffeeMethod : ToffeeField
    {
        public MethodInfo MethodInfo { get; private set; }
        public Type[] Parameters { get; private set; }

        public ToffeeMethod(ToffeeObject owner, MethodInfo methodInfo, ToffeeModifiers modifiers = ToffeeModifiers.None)
            : base(owner, methodInfo.Name, modifiers)
        {
            MethodInfo = methodInfo;
            Parameters = MethodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
        }

        public void Invoke(object instance, params object[] parameters)
        {            
            if (instance.GetType() != Owner.GetType())
                throw new ToffeeException("Tried to invoke method '{0}' on class '{1}' but the instance was not the correct type.", Identifier, Owner.Name);
            if (Parameters.Length != parameters.Length)
                throw new ToffeeException("Tried to invoke method '{0}' on class '{1}' but didn't get enough parameters.", Identifier, Owner.Name);
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].GetType() != Parameters[i])
                    throw new ToffeeException("Tried to invoke method '{0}' on class '{1}' but a parameter type was incorrect.", Identifier, Owner.Name);
            }
            MethodInfo.Invoke(instance, parameters);
        }

        public override FieldUpdate MakeFieldUpdate(params object[] parameters)
        {
            // Do we have the correct number of parameters?
            if (parameters.Length != Parameters.Length)
                throw new ToffeeException("Field update {0}::{1} requires {2} parameters, got {3}.",
                    Owner.Name, Identifier, Parameters.Length, parameters.Length);

            // Write the parameters to a temporary packet
            ToffeePacket packet = new ToffeePacket(Owner.Network);
            for (int i = 0; i < parameters.Length; i++)
            {
                // Is the parameter the correct Type?
                if (Parameters[i].GetType() != parameters[i].GetType())
                    throw new ToffeeException("Invalid parameter type for field update {0}::{1}", Identifier, Identifier);
                packet.Write(parameters[i]);
            }

            // Return the FieldUpdate
            return new FieldUpdate
            {
                FieldId = FieldId,
                Parameters = packet.GetBytes()
            };
        }
    }
}
