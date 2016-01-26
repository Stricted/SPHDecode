using System.IO;
using System.IO.Compression;

namespace SPHDecode.Implementations
{
    class Zlib
    {
        public static byte[] DecompressData(byte[] data)
        {
            MemoryStream ms = new MemoryStream();
            MemoryStream stream = new MemoryStream(data, 2, data.Length - 2);
            DeflateStream s = new DeflateStream(stream, CompressionMode.Decompress);
            s.CopyTo(ms);
            return ms.ToArray();

        }

        public static byte[] CompressData(byte[] data)
        {           
            MemoryStream ms = new MemoryStream();
            Ionic.Zlib.ZlibStream s = new Ionic.Zlib.ZlibStream(ms, Ionic.Zlib.CompressionMode.Compress, Ionic.Zlib.CompressionLevel.Level9);
            
            s.Write(data, 0, data.Length);
            s.Close();

            return ms.ToArray();
        }
    }
}
