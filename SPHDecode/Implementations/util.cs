using System;
using System.Xml;

namespace SPHDecode.Implementations
{
    class util
    {
        public static bool IsValidXML(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            if (value.EndsWith("\0"))
                value = value.Substring(0, value.Length - 1);

            try
            {
                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.LoadXml(value);

                return true;
            }
            catch (XmlException ex)
            {
                LogManager.WriteToLog(ex.Message);
                return false;
            }
        }

        public static byte[] removeNullByte (byte[] data)
        {
            if (data[data.Length - 1].Equals(0))
            {
                byte[] tmp = new byte[data.Length - 1];
                Array.Copy(data, tmp, data.Length - 1);

                return tmp;
            }

            return data;
        }

        public static byte[] addNullByte (byte[] data)
        {
            if (data[data.Length - 1].Equals(0).Equals(false))
            {
                byte[] tmp = data;
                Array.Resize(ref tmp, tmp.Length + 1);

                return tmp;
            }

            return data;
        }
    }
}
