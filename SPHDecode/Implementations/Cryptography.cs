using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml;

namespace SPHDecode.Implementations
{
    public static class Cryptography
    {
        private static byte[] KEY = new byte[] { 22, 39, 41, 141, 146, 199, 249, 4, 22, 135, 33, 125, 42, 121, 133, 198, 243, 104, 188, 35, 46, 48, 11, 1, 142, 200, 248, 130, 113, 81, 73, 62 }; // "1627298D92C7F9041687217D2A7985C6F368BC232E300B018EC8F8827151493E"
        private static byte[] IV = new byte[] { 89, 48, 127, 77, 236, 78, 199, 214, 97, 87, 151, 33, 145, 150, 117, 0 }; // "59307F4DEC4EC7D66157972191967500"

        public static string Decrypt(byte[] clearText)
        {
            string response = string.Empty;

            try
            {
                Aes encryptor = Aes.Create();

                if (Object.Equals(encryptor, null))
                {
                    return null;
                }

                encryptor.KeySize = 256;
                encryptor.BlockSize = 128;
                encryptor.Mode = CipherMode.CBC;
                encryptor.Padding = PaddingMode.Zeros;
                encryptor.Key = KEY;
                encryptor.IV = IV;

                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write);

                cs.Write(clearText, 0, clearText.Length);
                cs.Close();

                response = DecompressData(ms.ToArray());
            }
            catch (Exception ex)
            {
                LogManager.WriteToLog(ex.Message);
                new Thread(() => { MessageBox.Show("unable to decrypt config file", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error); }).Start();
                return null;
            }

            if (response.EndsWith("\0"))
                response = response.Substring(0, response.Length - 1);

            if (IsValidXML(response).Equals(false))
            {
                new Thread(() => { MessageBox.Show("Not a valid config file...", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error); }).Start();
                return string.Empty;
            }

            return response;
        }

        public static byte[] Encrypt(string data)
        {
            byte[] response = null;

            if (data.EndsWith("\0").Equals(false))
                data = string.Concat(data, "\0");

            if (IsValidXML(data).Equals(false))
            {
                new Thread(() => { MessageBox.Show("Not a valid config file...", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error); }).Start();
                return null;
            }

            byte[] clearText = Encoding.UTF8.GetBytes(data);

            try
            {
                byte[] comp = CompressData(clearText);
                Aes encryptor = Aes.Create();

                if (Object.Equals(encryptor, null))
                {
                    return null;
                }

                encryptor.KeySize = 256;
                encryptor.BlockSize = 128;
                encryptor.Mode = CipherMode.CBC;
                encryptor.Padding = PaddingMode.Zeros;
                encryptor.Key = KEY;
                encryptor.IV = IV;

                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write);

                cs.Write(comp, 0, comp.Length);
                cs.Close();

                response = ms.ToArray();
            }
            catch (Exception ex)
            {
                LogManager.WriteToLog(ex.Message);
                new Thread(() => { MessageBox.Show("unable to encrypt config file", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error); }).Start();
                return null;
            }

            return response;
        }

        private static bool IsValidXML(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            try
            {
                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.LoadXml(value);
                    
                return true;
            }
            catch (XmlException)
            {
                return false;
            }
        }

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
            Stream s = new zlib.ZOutputStream(ms, 9);
            s.Write(data, 0, data.Length);
            s.Close();

            return ms.ToArray();
        }
    }
}
