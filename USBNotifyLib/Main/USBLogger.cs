﻿using System;
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


        private static string LogPath => Path.Combine(_baseDir, "log.txt");

        private static string ErrorPath => Path.Combine(_baseDir, "error.txt");

        public static void Log(string log)
        {
            LogToFile(LogPath, log);
        }

        public static void Error(string error)
        {
            LogToFile(ErrorPath, error);
        }

        private readonly static object _locker = new object();
        static void LogToFile(string path, string log)
        {
            lock (_locker)
            {
                var l = Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine + log + Environment.NewLine;
                File.AppendAllText(path, l);
            }           
        }
    }
}
