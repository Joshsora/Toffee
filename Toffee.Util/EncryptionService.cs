using System;
using System.Linq;

namespace Toffee.Util
{
    public class EncryptionService
    {
        private byte[] CurrentKey { get; set; }
        private ulong _baseEncryptionKey { get; set; }
        public ulong BaseEncryptionKey
        {
            get
            {
                return _baseEncryptionKey;
            }

            set
            {
                _baseEncryptionKey = value;
                if (CurrentKey.Length == 8)
                    CurrentKey = BitConverter.GetBytes(value);
                else
                    Array.Copy(BitConverter.GetBytes(value), 0, CurrentKey, 0, 8);
            }
        }
        private long _lastServerTimestamp { get; set; }
        public long LastServerTimestamp
        {
            get
            {
                return _lastServerTimestamp;
            }

            set
            {
                _lastServerTimestamp = value;
                if (CurrentKey.Length == 8)
                {
                    CurrentKey = new byte[16];
                    Array.Copy(BitConverter.GetBytes(BaseEncryptionKey), 0, CurrentKey, 0, 8);
                }

                Array.Copy(BitConverter.GetBytes(value), 0, CurrentKey, 8, 8);
            }
        }

        public EncryptionService(ulong key)
        {
            CurrentKey = new byte[8];
            BaseEncryptionKey = key;
        }

        public byte[] Encrypt(byte[] data)
        {
            // Clone the key so that we don't modify it
            byte[] key = (byte[])CurrentKey.Clone();

            // Work out how much padding needs to be done
            int wholeIterations = data.Length / key.Length;
            byte padding = (byte)((key.Length * (wholeIterations + 1)) - data.Length);
            if (padding > 0)
                wholeIterations++;

            // Create a new array for our return value
            byte[] returnVal = new byte[data.Length + padding + 1];
            returnVal[0] = padding;

            // Add the padding bytes
            Random rnd = new Random();
            for (int y = 0; y < padding; y++)
                returnVal[y + 1] = (byte)rnd.Next(0, 255);

            // Add the data
            Array.Copy(data, 0, returnVal, padding + 1, data.Length);

            // Encrypt the data and padding bytes
            int z = 1;
            for (int i = 0; i < wholeIterations; i++)
            {
                for (int x = 0; x < key.Length; x++)
                {
                    returnVal[z] ^= key[x];
                    key[x] = returnVal[z++];
                }
            }
            return returnVal;
        }

        public byte[] Decrypt(byte[] data)
        {
            // Clone the key so that we don't modify it
            byte[] key = (byte[])CurrentKey.Clone();

            // Get the actual data length
            int dataLength = data.Length - 1;

            // Security check (padded correctly?)
            if (dataLength % key.Length != 0)
                return new byte[0];

            // Make a new array to build our return value
            byte[] returnVal = new byte[data.Length];
            returnVal[0] = data[0];

            // Go through the data
            int z = 1;
            for (int i = 0; i < (dataLength / key.Length); i++)
            {
                for (int x = 0; x < key.Length; x++)
                {
                    returnVal[z] = (byte)(data[z] ^ key[x]);
                    key[x] = data[z++];
                }
            }

            // Skip the padding bytes
            byte padding = returnVal[0];
            return returnVal.Skip(1 + padding).ToArray();
        }
    }
}
