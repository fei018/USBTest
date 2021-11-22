using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBNotifyLib
{
    public class UsbLogger
    {
        private static readonly string _baseDir = AppDomain.CurrentDomain.BaseDirectory;


        public static string LogPath => Path.Combine(_baseDir, "log.txt");

        public static string ErrorPath => Path.Combine(_baseDir, "error.txt");

        public static void LogTime()
        {
            
        }

        public static void Log(string log)
        {
            //ConsoleLog(log);
            LogToFile(LogPath, log);
        }

        public static void Error(string error)
        {
            var l = Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd HH:ss") + Environment.NewLine + error + Environment.NewLine;
            LogToFile(ErrorPath, l);
            //Console.WriteLine(l);
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

        static void LogToFile(string path, string log)
        {
            var l = Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine + log + Environment.NewLine;
            File.AppendAllText(path,l);
        }
    }
}
