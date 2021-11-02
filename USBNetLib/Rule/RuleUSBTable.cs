using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBNetLib
{
    public class RuleUSBTable
    {
        private static HashSet<RuleUSB> CacheTable { get; set; }

        private static readonly object _locker_CacheTable = new object();

        public RuleUSBTable()
        {
            CheckCacheTable();
        }

        #region + private void CheckCacheTable()
        private void CheckCacheTable()
        {
            if (CacheTable == null || CacheTable.Count <= 0)
            {
                Reload_RuleUSBTable();
            }
        }
        #endregion

        #region + public void Reload_RuleUSBTable()
        public void Reload_RuleUSBTable()
        {
            try
            {
                var table = USBConfig.Read_RuleUSBTable();
                if (table == null || table.Length <= 0)
                {
                    throw new Exception(USBConfig.RuleUSBTablePath + " file nothing ?");
                }

                var cache = new HashSet<RuleUSB>();

                foreach (var line in table)
                {
                    try
                    {
                        var data = Base64Decode(line);

                        if (data.Split(',').Length == 3)
                        {
                            string[] d = data.Split(',');
                            var vid = Convert.ToUInt16(d[0]?.Trim());
                            var pid = Convert.ToUInt16(d[1]?.Trim());
                            var serial = d[2]?.Trim();

                            var usb = new RuleUSB
                            {
                                Vid = vid,
                                Pid = pid,
                                SerialNumber = serial
                            };

                            cache.Add(usb);
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
                    if (t.Pid == usb.Pid && t.Vid == usb.Vid && t.SerialNumber == usb.SerialNumber)
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
