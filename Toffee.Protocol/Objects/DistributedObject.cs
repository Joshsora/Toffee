using Toffee.Protocol.Definitions;

namespace Toffee.Protocol.Objects
{
    public class DistributedObject
    {
        public ObjectContainer Container { get; private set; }
        public ToffeeClass Definition { get; private set; }
        public uint DistributedId { get; private set; }
        public bool IsOwned { get; internal set; }
        public bool IsFriend { get; internal set; }

        public virtual void Generate()
        {

        }

        public virtual void Destroy()
        {

        }

        public void SendUpdate(string fieldName, params object[] parameters)
        {
            // ToffeeClientPacket packet = Definition.PrepareFieldUpdate(
                // Container.Client, DistributedId, fieldName, parameters);
            // Container.Client.Send(packet);
        }
    }
}
