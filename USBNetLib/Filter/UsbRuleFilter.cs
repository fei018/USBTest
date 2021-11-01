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
    public partial class UsbRuleFilter
    {
        /// <summary>
        /// 
        /// </summary>
        private static List<RuleUSB> Table_RuleUSB;

        private static readonly object _locker_USBTable = new object();

        private readonly USBBusController _UsbBus;

        public UsbRuleFilter()
        {
            _UsbBus = new USBBusController();
        }

        #region + public void Filter_NotifyUSB_Use_DriveLetter(char driveLetter)
        public void Filter_NotifyUSB_Use_DriveLetter(char driveLetter)
        {
            try
            {
                var usb = Get_NotityUSb_DiskPath_by_DriveLetter_WMI(driveLetter);
                if (usb != null)
                {
                    Filter_NotifyUSB_Use_DiskPath(usb);
                }
                else
                {
                    throw new Exception("Cannot find disk by driveLetter: " + driveLetter);
                }
            }
            catch (Exception ex)
            {
                USBLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + public viod Filter_NotifyUSB_Use_DiskPath(NotifyUSB notifyUsb)
        /// <summary>
        /// 只需 notifyUsb.DiskPath 賦值
        /// </summary>
        /// <param name="notifyUsb"></param>
        public void Filter_NotifyUSB_Use_DiskPath(NotifyUSB notifyUsb)
        {
            try
            {
                if (Table_RuleUSB == null)
                {
                    Rule_FilterUSBTable();
                }

                if (!Find_UsbDeviceId_By_DiskPath_SetupDi(notifyUsb))
                {
                    Rule_NotFound_UsbDeviceID_By_DiskPath_SetupDi(notifyUsb);
                    return;
                }

                if (!_UsbBus.Find_VidPidSerial_In_UsbBus(notifyUsb))
                {
                    Rule_NotFound_NotityUSB_VidPidSerial_In_UsbBus(notifyUsb);
                    return;
                }

                lock (_locker_USBTable)
                {
                    if (Table_RuleUSB.Count <= 0)
                    {
                        Rule_FilterUSBTable();
                    }

                    foreach (var rule in Table_RuleUSB)
                    {
                        if (rule.IsMatchNotifyUSB(notifyUsb))
                        {
                            Rule_Match_In_FilterUSBTable(notifyUsb);
                            return;
                        }
                    }

                    Rule_NotMatch_In_FilterUSBTable(notifyUsb);
                }
            }
            catch (Exception ex)
            {
                USBLogger.Error(ex.Message);
                USBLogger.Error(notifyUsb.ToString());               
            }
        }
        #endregion

        #region + public void Filter_Scan_All_USB_Disk()
        /// <summary>
        /// 重新全局 scan usb disk to filter
        /// </summary>
        public void Filter_Scan_All_USB_Disk()
        {
            try
            {
                var usbList = Get_All_NotifyUSB_DiskPath_List_by_BusType_USB_WMI();
                if (usbList.Count > 0)
                {
                    foreach (var usb in usbList)
                    {
                        Filter_NotifyUSB_Use_DiskPath(usb);
                    }
                }
            }
            catch (Exception ex)
            {
                USBLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + public Set_Filter_USBTable()
        public void Set_Filter_USBTable()
        {
            try
            {
                var file = USBConfig.RuleUSBTablePath;
                if (!File.Exists(file))
                {
                    throw new Exception("FilterUSBTable.txt not exist.");
                }

                var lines = File.ReadAllLines(file);

                var table = new List<RuleUSB>();
                if (lines.Length > 0)
                {
                    foreach (var line in lines)
                    {
                        if (line.Split(',').Length == 3)
                        {
                            var vid = UInt16.Parse(line.Split(',')[0]?.Trim());
                            var pid = UInt16.Parse(line.Split(',')[1]?.Trim());
                            var serial = line.Split(',')[2]?.Trim();

                            var usb = new RuleUSB
                            {
                                Vid = vid,
                                Pid = pid,
                                SerialNumber = serial
                            };

                            table.Add(usb);
                        }
                    }
                }
                lock (_locker_USBTable)
                {
                    Table_RuleUSB = table;
                }
            }
            catch (Exception ex)
            {
                USBLogger.Error(ex.Message);
                throw;
            }
        }
        #endregion

        #region + public void UpdateUSBList_Timer()
        public void UpdateUSBList_Timer()
        {

        }
        #endregion


        //* Filter Rule *//

        #region + private void Rule_FilterUSBTable()
        private void Rule_FilterUSBTable()
        {
            if (Table_RuleUSB == null)
            {
                Set_Filter_USBTable();
            }
            if (Table_RuleUSB.Count <= 0)
            {
                // get table form server
            }
        }
        #endregion

        #region + private void Rule_NotFound_UsbDeviceID_By_DiskPath_SetupDi(NotifyUSB usb)
        /// <summary>
        /// 找不到 大多數係 非 USB device
        /// </summary>
        /// <param name="usb"></param>
        private void Rule_NotFound_UsbDeviceID_By_DiskPath_SetupDi(NotifyUSB usb)
        {
            USBLogger.Log("Not Found In SetupDi Usb DeviceId: " + usb.ToString());
        }
        #endregion

        #region + private void Rule_NotFound_NotityUSB_VidPidSerial_In_UsbBus(NotifyUSB notifyUsb)
        /// <summary>
        /// should not happen
        /// </summary>
        /// <param name="notifyUsb"></param>
        private void Rule_NotFound_NotityUSB_VidPidSerial_In_UsbBus(NotifyUSB notifyUsb)
        {
            // should not happen
        }
        #endregion

        #region + private void Rule_Match_In_FilterUSBTable(NotifyUSB usb)
        private void Rule_Match_In_FilterUSBTable(NotifyUSB usb)
        {
            try
            {
                Set_Disk_IsReadOnly_by_DiskPath_WMI(usb.DiskPath, false);
                USBLogger.Log("=== Match In USB ===");
                USBLogger.Log(usb.ToString());
                USBLogger.Log("------");
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region + private void Rule_NotMatch_In_FilterUSBTable(NotifyUSB usb)
        private void Rule_NotMatch_In_FilterUSBTable(NotifyUSB usb)
        {
            try
            {
                Set_Disk_IsReadOnly_by_DiskPath_WMI(usb.DiskPath, true);
                NotMatchSendMessage(usb);

                USBLogger.Log("=== Not Match USB ===");
                USBLogger.Log(usb.ToString());
                USBLogger.Log("Set Disk ReadOnly Success.");
                USBLogger.Log("------");
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        #endregion
    }
}
