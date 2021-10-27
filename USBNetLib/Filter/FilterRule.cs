using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Collections.Concurrent;
using System.IO;

namespace USBNetLib
{
    internal class RuleFilter
    {
        public static ConcurrentBag<RuleUSB> Filter_USBTable { get; set; }

        #region + Set_Filter_USBTable()
        public void Set_Filter_USBTable()
        {
            var list = new ConcurrentBag<RuleUSB>();

            var file = USBConfig.FilterUSBTablePath;
            if (!File.Exists(file))
            {
                return;
            }

            var lines = File.ReadAllLines(file);

            if (lines.Length <= 0) return;

            foreach (var line in lines)
            {
                if (line.Split(',').Length == 3)
                {
                    var vid = UInt16.Parse(line.Split(',')[0]);
                    var pid = UInt16.Parse(line.Split(',')[1]);
                    var serial = line.Split(',')[2];

                    var usb = new RuleUSB
                    {
                        Vid = vid,
                        Pid = pid,
                        SerialNumber = serial
                    };

                    list.Add(usb);
                }
            }

            Filter_USBTable = list;
        }

        #endregion

        #region + public void UpdateUSBList_Timer()
        public void UpdateUSBList_Timer()
        {

        }
        #endregion

        #region + public bool Filter_NotifyUSB(NotifyUSB notifyUsb)
        private static object _locker = new object();

        public bool Filter_NotifyUSB(NotifyUSB notifyUsb)
        {
            lock (_locker)
            {
                if (Filter_USBTable == null)
                {
                    throw new Exception("RuleTable.USBTable is Null.");
                }

                foreach (RuleUSB f in Filter_USBTable)
                {
                    if (f.IsMatchNotifyUSB(notifyUsb))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        #endregion


        #region + public void NotFound_UsbDeviceID_By_DiskPath_SetupDi(string diskPath)
        public void NotFound_UsbDeviceID_By_DiskPath_SetupDi(string diskPath)
        {

        }
        #endregion

        #region + public void NotFound_NotityUSB_VidPidSerial_In_UsbBus(NotifyUSB notifyUsb)
        public void NotFound_NotityUSB_VidPidSerial_In_UsbBus(NotifyUSB notifyUsb)
        {

        }
        #endregion

        #region + public void NotMatch_In_FilterUSBTable(NotifyUSB usb)
        public void NotMatch_In_FilterUSBTable(NotifyUSB usb)
        {
            USBLogger.Log("Usb no match in policy table:");
            USBLogger.Log(usb.ToString());

            try
            {
                Set_Disk_IsReadOnly(usb.DiskPath,true);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region + private void Set_Disk_IsReadOnly(string diskPath, bool isReadOnly)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="diskPath"></param>
        /// <param name="isReadOnly"></param>
        /// <exception cref="no admin right throw error access denied"></exception>
        private void Set_Disk_IsReadOnly(string diskPath, bool isReadOnly)
        {
            try
            {
                using (ManagementObject disk = Get_Disk_WMI_By_Path(diskPath))
                {
                    if (disk == null) return;

                    bool IsReadOnly = bool.Parse(disk["IsReadOnly"].ToString());
                    bool IsSystem = bool.Parse(disk["IsSystem"].ToString());
                    bool IsBoot = bool.Parse(disk["IsBoot"].ToString());

                    if (!IsReadOnly && !IsBoot && !IsSystem)
                    {
                        var inParams = disk.GetMethodParameters("SetAttributes");
                        inParams["IsReadOnly"] = isReadOnly;
                        var r = disk.InvokeMethod("SetAttributes", inParams, null)["ReturnValue"].ToString();
                        USBLogger.Log(r);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private ManagementObject Get_Disk_WMI_By_Path(string diskPath)
        {
            string q = diskPath.TrimStart('\\','\\','?','\\');
            var scope = new ManagementScope(@"\\.\ROOT\Microsoft\Windows\Storage");
            var query = new ObjectQuery($"SELECT * FROM MSFT_Disk WHERE Path LIKE '%{q}'" );
            var searcher = new ManagementObjectSearcher(scope, query);
            var disks = searcher.Get();

            if (disks.Count == 1)
            {
                foreach (ManagementObject d in disks)
                {
                    return d;
                }
            }
            return null;
        }
        #endregion
    }
}
