using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBNetLib
{
    public class USBLogger
    {
        public static void Log()
        {

        }

        public static void Log(string log)
        {
            
            LogToFile(log);
        }


        static void ConsoleLog(string log)
        {
            if (string.IsNullOrEmpty(log))
            {
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine(log);
            }
        }

        static void LogToFile(string log)
        {
            var l = Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd HH:ss") + Environment.NewLine + log;
            File.AppendAllText(USBConfig.LogFile,l);
        }
    }
}
