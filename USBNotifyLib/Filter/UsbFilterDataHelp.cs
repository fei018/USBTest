using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBCommon;

namespace USBNotifyLib
{
    public class UsbFilterDataHelp
    {
        private static string _UsbFilterDataFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"usbfilter.dat");

        private static HashSet<string> CacheDb { get; set; }

        private static readonly object _locker_CacheDb = new object();


        #region + private void CheckCacheDb()
        private static void CheckCacheDb()
        {
            if (CacheDb == null || CacheDb.Count <= 0)
            {
                Reload_UsbFilterData();
            }
        }
        #endregion

        #region + public void Reload_UsbFilterData()
        public static void Reload_UsbFilterData()
        {
            try
            {
                var table = ReadFile_UsbFilterData();
                if (table == null || table.Length <= 0)
                {
                    throw new Exception(_UsbFilterDataFile + " file is null or empty. ?");
                }

                var cache = new HashSet<string>();

                foreach (var line in table)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var data = Base64CodeHelp.Base64Decode(line.Trim());
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

        #region + public bool IsFind(UsbDisk usb)
        public static bool IsFind(UsbDisk usb)
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

        #region + public static void Set_UsbFilterData_byHttp(UsbFilterDbHttp setting)
        public static void Set_UsbFilterData_byHttp(string usbFilterData)
        {
            try
            {
                WriteFile_UsbFilterData(usbFilterData);
                Reload_UsbFilterData();
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + private static string[] ReadFile_UsbFilterData()
        private static readonly object _locker_UsbFilterDb = new object();
        private static string[] ReadFile_UsbFilterData()
        {
            lock (_locker_UsbFilterDb)
            {
                try
                {
                    if (!File.Exists(_UsbFilterDataFile))
                    {
                        new AgentHttpHelp().GetUsbFilterData_Http();
                    }

                    if (File.Exists(_UsbFilterDataFile))
                    {
                        return File.ReadAllLines(_UsbFilterDataFile, new UTF8Encoding(false));
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

        #region + private static void WriteFile_UsbFilterData(string txt)
        private static void WriteFile_UsbFilterData(string txt)
        {
            lock (_locker_UsbFilterDb)
            {
                File.WriteAllText(_UsbFilterDataFile, txt, new UTF8Encoding(false));
            }
        }
        #endregion
    }
}
