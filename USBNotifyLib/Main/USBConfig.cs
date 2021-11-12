using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;

namespace USBNotifyLib
{
    public class UsbConfig
    {
        private static readonly string _baseDir = AppDomain.CurrentDomain.BaseDirectory;

        private static string _configJson = Path.Combine(_baseDir, "appcfg.json");

        public static string LogPath => Path.Combine(_baseDir, "log.txt");

        public static string ErrorPath => Path.Combine(_baseDir, "error.txt");

        public static string USBNotifyFilter => Path.Combine(_baseDir, "usbnfilter.exe");

        public static string USBNotifyDesktop => Path.Combine(_baseDir, "usbndesktop.exe");

        public static string UsbFilterDbPath => (string)AppConfig()["path"]["usbFilterDb"];

        public static string UsbFilterDbUrl => (string)AppConfig()["url"]["usbFilterDb"];

        public static int UpdateTimer => (int)AppConfig()["updateTimer"];

        public static string PostComputerInfoUrl => (string)AppConfig()["url"]["postComputerInfo"];

        public static string PostComUsbHistoryInfoUrl => (string)AppConfig()["url"]["postComUsbHistoryInfo"];


        #region + public static string[] ReadFile_UsbFilterDb()
        private static readonly object _locker_UsbFilterDb = new object();
        public static string[] ReadFile_UsbFilterDb()
        {
            lock (_locker_UsbFilterDb)
            {
                if (File.Exists(UsbFilterDbPath))
                {
                    return File.ReadAllLines(UsbFilterDbPath, Encoding.UTF8);
                }
                return null;
            }
        }
        #endregion

        #region + public static void WriteFile_UsbFilterDb(string txt)
        public static void WriteFile_UsbFilterDb(string txt)
        {
            lock (_locker_UsbFilterDb)
            {
                if (File.Exists(UsbFilterDbPath))
                {
                    File.WriteAllText(UsbFilterDbPath, txt, Encoding.UTF8);
                }
            }
        }
        #endregion

        #region + private static JObject GetConfigJsonObject()
        private static readonly object _Locker_Config = new object();
        private static JObject AppConfig()
        {
            try
            {
               
                lock (_Locker_Config)
                {
                    string config = null;
                    if (File.Exists(_configJson))
                    {
                        config = File.ReadAllText(_configJson);
                    }
                    return JObject.Parse(config);
                }               
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

    }
}
