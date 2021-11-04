using System;
using System.IO;
using System.Text;

namespace USBNetLib
{
    public class UsbConfig
    {
        private static readonly string _baseDir = AppDomain.CurrentDomain.BaseDirectory;
        private static string _config = Path.Combine(_baseDir, "app.cfg");

        public static string LogPath => Path.Combine(_baseDir, "log.txt");

        public static string ErrorPath => Path.Combine(_baseDir, "error.txt");

        public static string NUWAppPath => Path.Combine(_baseDir, "nuwapp.exe");

        public static string FilterUSBTablePath => GetConfigValue("usbfiltertable");

        public static string GetUsbFilterUrl => GetConfigValue("updateusbfilterurl");

        public static int UpdateTimer => Convert.ToInt32(GetConfigValue("updatetimer"));

        public static string PostComputerInfoUrl => GetConfigValue("postcomurl");

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

        #region + public static string[] Read_FilterUSBTable()
        private static readonly object _locker_Table = new object();
        public static string[] Read_FilterUSBTable()
        {
            lock (_locker_Table)
            {
                if (File.Exists(FilterUSBTablePath))
                {
                    return File.ReadAllLines(FilterUSBTablePath, Encoding.UTF8);
                }
                return null;
            }
        }
        #endregion

        #region + public static void Write_FilterUSbTable(string txt)
        public static void Write_FilterUSbTable(string txt)
        {
            lock (_locker_Table)
            {
                if (File.Exists(FilterUSBTablePath))
                {
                    File.WriteAllText(FilterUSBTablePath, txt, Encoding.UTF8);
                }
            }
        }
        #endregion

    }
}
