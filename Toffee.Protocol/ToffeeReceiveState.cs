namespace Toffee.Protocol
{
    /// <summary>
    /// The state used during asyncronous operations to receive data.
    /// </summary>
    public class ToffeeReceiveState
    {
        /// <summary>
        /// The buffer of data received.
        /// </summary>
        public byte[] Buffer { get; private set; }

        /// <summary>
        /// How much data has been received so far.
        /// </summary>
        public int ReceivedLength { get; set; }

        /// <summary>
        /// How much data we're still expecting.
        /// </summary>
        public int AwaitingLength
        {
            get
            {
                return (Buffer.Length - ReceivedLength);
            }
        }

        /// <summary>
        /// True if the entire data has been received.
        /// </summary>
        public bool Received
        {
            get
            {
                return (ReceivedLength == Buffer.Length);
            }
        }

        /// <summary>
        /// Creates a new ToffeeReceiveState.
        /// </summary>
        /// <param name="length">The length that we're expecting.</param>
        public ToffeeReceiveState(int length)
        {
            Buffer = new byte[length];
            ReceivedLength = 0;
        }
    }
}
