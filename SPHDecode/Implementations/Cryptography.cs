using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace SPHDecode.Implementations
{
    public static class Cryptography
    {
		private static byte[][] KEY = new byte[][] {
			new byte[] { 22, 39, 41, 141, 146, 199, 249, 4, 22, 135, 33, 125, 42, 121, 133, 198, 243, 104, 188, 35, 46, 48, 11, 1, 142, 200, 248, 130, 113, 81, 73, 62 }, /* v2 "1627298D92C7F9041687217D2A7985C6F368BC232E300B018EC8F8827151493E" */
			new byte[] { 221, 31, 187, 117, 150, 208, 213, 139, 191, 51, 106, 186, 57, 19, 102, 237, 62, 215, 70, 8, 73, 187, 158, 250, 185, 100, 156, 153, 152, 99, 191, 37 }, /* v3 "DD1FBB7596D0D58BBF336ABA391366ED3ED7460849BB9EFAB9649C999863BF25" */
		};

		private static byte[][] IV = new byte[][] {
			new byte[] { 89, 48, 127, 77, 236, 78, 199, 214, 97, 87, 151, 33, 145, 150, 117, 0 }, /* v2 "59307F4DEC4EC7D66157972191967500" */
			new byte[] { 179, 128, 225, 122, 12, 71, 110, 138, 72, 11, 85, 37, 58, 186, 230, 102 }, /* v3 "B380E17A0C476E8A480B55253ABAE666" */
		};

		public static byte[] Decrypt(byte[] clearText)
        {
            byte[] response = new byte[0];

			byte[] data;
			for (int x = 0; x < KEY.Length; x++)
			{
				try
				{
					byte[] key = KEY[x];
					byte[] iv = IV[x];
					data = AESHelper(key, iv, clearText, true);
					var d = Zlib.DecompressData(data);
					Array.Resize(ref response, d.Length);
					Array.Copy(d, response, d.Length);
				}
				catch (Exception)
				{
					continue;
				}

				break;
			}

			if (response.Length == 0)
			{
				MessageBox.Show("unable to decrypt config file", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error);
				return null;
			}


            if (util.IsValidXML(response).Equals(false))
            {
                MessageBox.Show("Not a valid config file...", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            
            return response;
        }

        public static byte[] Encrypt(byte[] data)
        {
            byte[] response = null;

            if (util.IsValidXML((data)).Equals(false))
            {
                MessageBox.Show("Not a valid config file...", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            byte[] clearText = data;

            try
            {
                byte[] comp = Zlib.CompressData(clearText);

				response = AESHelper(KEY[0], IV[0], comp); // use v2 key, v3 can read configs that are encrypted with the v2 key
            }
            catch (Exception ex)
            {
                LogManager.WriteToLog(ex.ToString());
                MessageBox.Show("unable to encrypt config file", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            return response;
        }

        private static byte[] AESHelper (byte[] key, byte[] iv, byte[] data, bool decrypt = false)
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
            encryptor.Key = key;
            encryptor.IV = iv;

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write);

            if (decrypt)
                cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write);
            
            cs.Write(data, 0, data.Length);
            cs.Close();

            return ms.ToArray();
        }


    }
}
