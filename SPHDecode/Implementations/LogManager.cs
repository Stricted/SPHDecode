using System;
using System.Threading;
using System.IO;

namespace SPHDecode.Implementations
{
    public class LogManager
    {
        public static void WriteToLog(string value)
        {
            DateTime time = DateTime.Now;
            string format = "dd.MM.yyyy HH:mm:ss";

            Console.WriteLine(string.Concat("[", time.ToString(format), "]: ", value));
            new Thread(() => {
                try
                {
                    StreamWriter file = new StreamWriter("ErrorLog.txt", true);
                    file.WriteLine(string.Concat("[", time.ToString(format), "]: ", value));
                    file.Close();
                }
                catch (Exception ex)
                {
                    // nothing
                    Console.WriteLine(ex.Message);
                }
            }).Start();
        }
    }
}
