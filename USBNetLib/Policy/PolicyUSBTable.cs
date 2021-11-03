using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBNetLib
{
    public class PolicyUSBTable
    {
        private static HashSet<string> CacheTable { get; set; }

        private static readonly object _locker_CacheTable = new object();

        public PolicyUSBTable()
        {
            CheckCacheTable();
        }

        #region + private void CheckCacheTable()
        private void CheckCacheTable()
        {
            if (CacheTable == null || CacheTable.Count <= 0)
            {
                Reload_PolicyUSBTable();
            }
        }
        #endregion

        #region + public void Reload_PolicyUSBTable()
        public void Reload_PolicyUSBTable()
        {
            try
            {
                var table = USBConfig.Read_PolicyUSBTable();
                if (table == null || table.Length <= 0)
                {
                    throw new Exception(USBConfig.RuleUSBTablePath + " file nothing ?");
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

                lock (_locker_CacheTable)
                {
                    CacheTable = cache;
                }
            }
            catch (Exception ex)
            {
                USBLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + public bool IsFind(NotifyUSB usb)
        public bool IsFind(NotifyUSB usb)
        {
            if (CacheTable != null && CacheTable.Count > 0)
            {
                foreach (var t in CacheTable)
                {
                    if (t.ToLower() == usb.ToPolicyString().ToLower())
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
