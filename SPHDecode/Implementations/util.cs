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
    }
}
