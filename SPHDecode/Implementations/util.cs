using System;
using System.Text;
using System.Xml;

namespace SPHDecode.Implementations
{
    class util
    {
        public static bool IsValidXML(byte[] data)
        {
            if (Object.Equals(data, null))
            {
                return false;
            }

            try
            {
                string value = Encoding.UTF8.GetString(data);

                if (string.IsNullOrWhiteSpace(value))
                    return false;

                if (value.EndsWith("\0"))
                    value = value.Substring(0, value.Length - 1);

                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.LoadXml(value);

                return true;
            }
            catch (XmlException ex)
            {
                LogManager.WriteToLog(ex.ToString());
                return false;
            }
        }

        public static byte[] removeNullByte (byte[] data)
        {
            
            if (Object.Equals(data, null).Equals(false) && data[data.Length - 1].Equals(0))
            {
                byte[] tmp = new byte[data.Length - 1];
                Array.Copy(data, tmp, data.Length - 1);

                return tmp;
            }

            return data;
        }

        public static byte[] addNullByte (byte[] data)
        {
            if (Object.Equals(data, null).Equals(false) && data[data.Length - 1].Equals(0).Equals(false))
            {
                byte[] tmp = data;
                Array.Resize(ref tmp, tmp.Length + 1);

                return tmp;
            }

            return data;
        }
    }
}
