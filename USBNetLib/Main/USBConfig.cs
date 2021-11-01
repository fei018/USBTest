﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace USBNetLib
{
    public class USBConfig
    {
        private static readonly string _baseDir = AppDomain.CurrentDomain.BaseDirectory;
        private static string _config = Path.Combine(_baseDir, "app.cfg");

        public static string LogPath => Path.Combine(_baseDir, "log.txt");

        public static string ErrorPath => Path.Combine(_baseDir, "error.txt");

        public static string NUWAppServicePath => Path.Combine(_baseDir, "nuwapps.exe");

        public static string RuleUSBTablePath => GetConfigValue("usbruletable");
    

        public static string UpdateUrl => GetConfigValue("updateurl");


        private static string GetConfigValue(string arg)
        {
            try
            {
                if (File.Exists(_config))
                {
                    var lines = File.ReadAllLines(_config);
                    foreach (var l in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(l))
                        {
                            var a = l.Split('=')[0].Trim().ToLower();
                            var v = l.Split('=')[1].Trim();
                            if (a == arg.ToLower())
                            {
                                return v;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        
    }
}
