using System.IO;
using System.IO.Compression;
using zlib;

namespace SPHDecode.Implementations
{
    class GZip
    {
        public static string DecompressData(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data, 2, data.Length - 2);
            DeflateStream inflater = new DeflateStream(stream, CompressionMode.Decompress);
            StreamReader streamReader = new StreamReader(inflater);

            return streamReader.ReadToEnd();
        }

        public static byte[] CompressData(byte[] data)
        {
            MemoryStream ms = new MemoryStream();
            Stream s = new ZOutputStream(ms, 9);
            s.Write(data, 0, data.Length);
            s.Close();

            return ms.ToArray();
        }
    }
}
