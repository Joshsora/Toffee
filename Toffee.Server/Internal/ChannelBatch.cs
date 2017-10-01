namespace Toffee.Server.Internal
{
    /// <summary>
    /// A batch of allocated channels.
    /// </summary>
    public class ChannelBatch
    {
        public uint Min { get; private set; }
        public uint Max { get; private set; }
        private bool[] Allocated { get; set; }

        public ChannelBatch(uint min, uint max)
        {
            Min = min;
            Max = max;
            Allocated = new bool[Max - Min];
        }

        public bool TryAllocate(out uint channel)
        {
            // Start the channel at the minimum it can be
            channel = Min;
            for (int i = 0; i < Allocated.Length; i++)
            {
                // Is this channel available?
                if (!Allocated[i])
                {
                    // Allocate the channel
                    Allocated[i] = true;
                    return true;
                }

                // Move to the next channel
                channel++;
            }

            // We're fully allocated.
            return false;
        }

        public void Free(uint channel)
        {
            // Is the given channel in the range?
            if ((channel < Min) || (channel > Max))
                return;

            // Work out the index, and then deallocate it
            int index = (int)(channel - Min);
            Allocated[index] = false;
        }
    }
}
