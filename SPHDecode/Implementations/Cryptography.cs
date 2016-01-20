using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
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
                byte[] data = AESHelper(clearText, true);

                response = GZip.DecompressData(data);
            }
            catch (Exception ex)
            {
                LogManager.WriteToLog(ex.Message);
                MessageBox.Show("unable to decrypt config file", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            if (response.EndsWith("\0"))
                response = response.Substring(0, response.Length - 1);

            if (util.IsValidXML(response).Equals(false))
            {
                MessageBox.Show("Not a valid config file...", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }

            return response;
        }

        public static byte[] Encrypt(string data)
        {
            byte[] response = null;

            if (util.IsValidXML(data).Equals(false))
            {
                MessageBox.Show("Not a valid config file...", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            if (data.EndsWith("\0").Equals(false))
                data = string.Concat(data, "\0");

            byte[] clearText = Encoding.UTF8.GetBytes(data);

            try
            {
                byte[] comp = GZip.CompressData(clearText);

                response = AESHelper(comp);
            }
            catch (Exception ex)
            {
                LogManager.WriteToLog(ex.Message);
                MessageBox.Show("unable to encrypt config file", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            return response;
        }

        private static byte[] AESHelper (byte[] data, bool decrypt = false)
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
            CryptoStream cs;
            if (decrypt)
            {
                cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write);
            }
            else {
                cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write);
            }

            cs.Write(data, 0, data.Length);
            cs.Close();

            return ms.ToArray();
        }


    }
}
