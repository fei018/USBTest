using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBCommon;

namespace USBNotifyLib
{
    public class UsbFilterDbHelp
    {
        private static string _UsbFilterDbFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "usbFilterDb.dat");

        private static HashSet<string> CacheDb { get; set; }

        private static readonly object _locker_CacheDb = new object();


        #region + private void CheckCacheDb()
        private static void CheckCacheDb()
        {
            if (CacheDb == null || CacheDb.Count <= 0)
            {
                Reload_UsbFilterDb();
            }
        }
        #endregion

        #region + public void Reload_UsbFilterDb()
        public static void Reload_UsbFilterDb()
        {
            try
            {
                var table = ReadFile_UsbFilterDb();
                if (table == null || table.Length <= 0)
                {
                    throw new Exception(_UsbFilterDbFile + " file nothing ?");
                }

                var cache = new HashSet<string>();

                foreach (var line in table)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            var data = Base64Decode(line.Trim());
                            cache.Add(data);
                        }
                    }
                    catch (Exception) { }
                }

                lock (_locker_CacheDb)
                {
                    CacheDb = cache;
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + public bool IsFind(NotifyUSB usb)
        public static bool IsFind(NotifyUsb usb)
        {
            CheckCacheDb();

            if (CacheDb != null && CacheDb.Count > 0)
            {
                foreach (var t in CacheDb)
                {
                    if (t.ToLower() == usb.UsbIdentity)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
        #endregion

        #region Base64Encode
        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        #endregion

        #region + public static void Set_UsbFilterDb_byHttp(UsbFilterDbHttp setting)
        public static void Set_UsbFilterDb_byHttp(GetUsbFilterDbHttp setting)
        {
            try
            {
                if (setting.UserUsbFilterEnabled)
                {
                    WriteFile_UsbFilterDb(setting.UsbFilterDb);
                    UsbRegistry.UsbFilterEnabled = setting.UserUsbFilterEnabled;
                    UsbFilter.IsEnable = setting.UserUsbFilterEnabled;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + private static string[] ReadFile_UsbFilterDb()
        private static readonly object _locker_UsbFilterDb = new object();
        private static string[] ReadFile_UsbFilterDb()
        {
            lock (_locker_UsbFilterDb)
            {
                try
                {
                    if (!File.Exists(_UsbFilterDbFile))
                    {
                        UsbHttpHelp.GetUsbFilterDb_Http();
                    }

                    if (File.Exists(_UsbFilterDbFile))
                    {
                        return File.ReadAllLines(_UsbFilterDbFile, Encoding.UTF8);
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

        #region + private static void WriteFile_UsbFilterDb(string txt)
        private static void WriteFile_UsbFilterDb(string txt)
        {
            lock (_locker_UsbFilterDb)
            {
                File.WriteAllText(_UsbFilterDbFile, txt, Encoding.UTF8);
            }
        }
        #endregion
    }
}
