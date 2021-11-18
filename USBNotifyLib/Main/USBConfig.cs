using Microsoft.Win32;
using System;
using System.IO;
using System.Text;

namespace USBNotifyLib
{
    public class UsbConfig
    {
        private const string _registryKey = "SOFTWARE\\Hiphing\\USBNotify";

        private static readonly string _baseDir = AppDomain.CurrentDomain.BaseDirectory;


        public static string LogPath => Path.Combine(_baseDir, "log.txt");

        public static string ErrorPath => Path.Combine(_baseDir, "error.txt");

        public static string UsbFilterDbFile => Path.Combine(_baseDir, "usbFilterDb.dat");

        // Registry

        public static bool UsbFilterEnabled {get;set;}

        public static string UsbFilterDbUrl { get; set; }

        public static int UpdateTimer { get; set; }

        public static string PostComputerInfoUrl { get; set; }

        public static string PostComUsbHistoryInfoUrl { get; set; }


        #region + public static string[] ReadFile_UsbFilterDb()
        private static readonly object _locker_UsbFilterDb = new object();
        public static string[] ReadFile_UsbFilterDb()
        {
            lock (_locker_UsbFilterDb)
            {
                try
                {
                    if (!File.Exists(UsbFilterDbFile))
                    {
                        UsbHttpHelp.GetUsbFilterDb_Http();
                    }

                    if (File.Exists(UsbFilterDbFile))
                    {
                        return File.ReadAllLines(UsbFilterDbFile, Encoding.UTF8);
                    }
                    return null;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
        #endregion

        #region + public static void WriteFile_UsbFilterDb(string txt)
        public static void WriteFile_UsbFilterDb(string txt)
        {
            lock (_locker_UsbFilterDb)
            {
                if (File.Exists(UsbFilterDbFile))
                {
                    File.WriteAllText(UsbFilterDbFile, txt, Encoding.UTF8);
                }
            }
        }
        #endregion

        #region + public static void ReloadRegistryConfig()
        public static void ReloadRegistryConfig()
        {
            UsbFilterEnabled = Convert.ToBoolean(ReadRegistryKey("UsbFilterEnabled"));
            UsbFilterDbUrl = ReadRegistryKey("UsbFilterDbUrl");
            UpdateTimer = Convert.ToInt32(ReadRegistryKey("UpdateTimer"));
            PostComputerInfoUrl = ReadRegistryKey("PostComputerInfoUrl");
            PostComUsbHistoryInfoUrl = ReadRegistryKey("PostComUsbHistoryInfoUrl");
        }
        #endregion

        #region + private string ReadRegistryKey(string name)
        private static string ReadRegistryKey(string name)
        {
            try
            {
                using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    using (var usbKey = hklm.OpenSubKey(_registryKey))
                    {
                        var value = usbKey.GetValue(name) as string;
                        return value;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

    }
}
