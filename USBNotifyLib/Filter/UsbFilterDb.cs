using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBNotifyLib
{
    public class UsbFilterDb
    {
        private static HashSet<string> CacheDb { get; set; }

        private static readonly object _locker_CacheDb = new object();

        public UsbFilterDb()
        {
            CheckCacheDb();
        }

        #region + private void CheckCacheDb()
        private void CheckCacheDb()
        {
            if (CacheDb == null || CacheDb.Count <= 0)
            {
                Reload_UsbFilterDb();
            }
        }
        #endregion

        #region + public void Reload_UsbFilterDb()
        public void Reload_UsbFilterDb()
        {
            try
            {
                var table = UsbConfig.ReadFile_UsbFilterDb();
                if (table == null || table.Length <= 0)
                {
                    throw new Exception(UsbConfig.UsbFilterDbPath + " file nothing ?");
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
        public bool IsFind(NotifyUsb usb)
        {
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
        private string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        #endregion
    }
}
