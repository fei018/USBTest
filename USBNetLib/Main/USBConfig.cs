using System;
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

        public static string NUWAppPath => Path.Combine(_baseDir, "nuwapp.exe");

        public static string RuleUSBTablePath => GetConfigValue("usbruletable");    

        public static string UpdateUrl => GetConfigValue("updateurl");

        public static int UpdateTimer => Convert.ToInt32(GetConfigValue("updatetimer"));



        #region + private static string GetConfigValue(string arg)
        private static readonly object _Locker_Config = new object();
        private static string GetConfigValue(string arg)
        {
            try
            {
                lock (_Locker_Config)
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
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region + public static string[] Read_RuleUSBTable()
        private static readonly object _locker_Table = new object();
        public static string[] Read_RuleUSBTable()
        {
            lock (_locker_Table)
            {
                if (File.Exists(RuleUSBTablePath))
                {
                    return File.ReadAllLines(RuleUSBTablePath);
                }
                return null;
            }
        }
        #endregion

        #region + public static void Write_RuleUSbTable(string txt)
        public static void Write_RuleUSbTable(string txt)
        {
            lock (_locker_Table)
            {
                if (File.Exists(RuleUSBTablePath))
                {
                    File.WriteAllText(RuleUSBTablePath,txt);
                }
            }
        }
        #endregion

    }
}
