using System.Text;

namespace Toffee.Util
{
    public static class CRC
    {
        private static ushort[] CRC16Table;
        private static uint[] CRC32Table;
        private static ulong[] CRC64Table;

        static CRC()
        {
            MakeCRC16Table();
            MakeCRC32Table();
            MakeCRC64Table();
        }

        public static ushort CalculateCRC16(byte[] bytes)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(((crc) & 0xff) ^ bytes[i]);
                crc = (ushort)((crc >> 8) ^ CRC16Table[index]);
            }
            return (ushort)~crc;
        }

        public static ushort CalculateCRC16(string str)
        {
            return CalculateCRC16(Encoding.UTF8.GetBytes(str));
        }

        public static uint CalculateCRC32(byte[] bytes)
        {
            uint crc = 0xFFFFFFFF;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(((crc) & 0xff) ^ bytes[i]);
                crc = (uint)((crc >> 8) ^ CRC32Table[index]);
            }
            return ~crc;
        }

        public static uint CalculateCRC32(string str)
        {
            return CalculateCRC32(Encoding.UTF8.GetBytes(str));
        }

        public static ulong CalculateCRC64(byte[] bytes)
        {
            ulong crc = 0xFFFFFFFFFFFFFFFF;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(((crc) & 0xff) ^ bytes[i]);
                crc = ((crc >> 8) ^ CRC64Table[index]);
            }
            return ~crc;
        }

        public static ulong CalculateCRC64(string str)
        {
            return CalculateCRC64(Encoding.UTF8.GetBytes(str));
        }

        public static void MakeCRC16Table()
        {
            ushort poly = CRCGlobals.CRC16Polynomial;
            CRC16Table = new ushort[256];
            for (ushort i = 0; i < 256; i++)
            {
                ushort r = i;
                for (int j = 0; j < 8; j++)
                    if ((r & 1) != 0)
                        r = (ushort)((r >> 1) ^ poly);
                    else
                        r >>= 1;
                CRC16Table[i] = r;
            }
        }

        public static void MakeCRC32Table()
        {
            uint poly = CRCGlobals.CRC32Polynomial;
            CRC32Table = new uint[256];
            for (uint i = 0; i < 256; i++)
            {
                uint r = i;
                for (int j = 0; j < 8; j++)
                    if ((r & 1) != 0)
                        r = (r >> 1) ^ poly;
                    else
                        r >>= 1;
                CRC32Table[i] = r;
            }
        }

        public static void MakeCRC64Table()
        {
            ulong poly = CRCGlobals.CRC64Polynomial;
            CRC64Table = new ulong[256];
            for (ulong i = 0; i < 256; i++)
            {
                ulong r = i;
                for (int j = 0; j < 8; j++)
                    if ((r & 1) != 0)
                        r = (r >> 1) ^ poly;
                    else
                        r >>= 1;
                CRC64Table[i] = r;
            }
        }
    }
}
