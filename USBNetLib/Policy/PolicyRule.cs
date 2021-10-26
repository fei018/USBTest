using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace USBNetLib
{
    internal class PolicyRule
    {
        public void NotFound_UsbDeviceID_By_DiskPath_SetupDi(string diskPath)
        {

        }

        public void NotFound_NotityUSB_VidPidSerial_In_UsbBus(NotifyUSB notifyUsb)
        {

        }

        #region 
        public void NotMatch_In_PolicyTable(NotifyUSB usb)
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
