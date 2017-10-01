using System.IO;
using Ionic.Zlib;

namespace Toffee.Util
{
    public static class CompressionService
    {
        public static byte[] Compress(byte[] data)
        {
            MemoryStream dataStream = new MemoryStream(data);
            MemoryStream compressed = new MemoryStream();
            using (ZlibStream zlibStream = new ZlibStream(compressed, CompressionMode.Compress))
            {
                dataStream.CopyTo(zlibStream);
            }
            return compressed.ToArray();
        }

        public static byte[] Decompress(byte[] data)
        {
            MemoryStream dataStream = new MemoryStream(data);
            MemoryStream decompressed = new MemoryStream();
            using (ZlibStream zlibStream = new ZlibStream(dataStream, CompressionMode.Decompress))
            {
                zlibStream.CopyTo(decompressed);
            }
            return decompressed.ToArray();
        }
    }
}
