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
    internal partial class UsbRuleFilter
    {
        /// <summary>
        /// 
        /// </summary>
        private static List<RuleUSB> Filter_USBTable;

        private static readonly object _locker_USBTable = new object();

        private readonly USBBusController _UsbBus;

        public UsbRuleFilter()
        {
            _UsbBus = new USBBusController();
        }

        #region MyRegion
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
                if (Filter_USBTable == null)
                {
                    throw new Exception("RuleTable.USBTable is Null.");
                }

                if (!Find_UsbDeviceId_By_DiskPath_SetupDi(notifyUsb))
                {
                    NotFound_UsbDeviceID_By_DiskPath_SetupDi(notifyUsb);
                    return;
                }

                if (!_UsbBus.Find_VidPidSerial_In_UsbBus(notifyUsb))
                {
                    NotFound_NotityUSB_VidPidSerial_In_UsbBus(notifyUsb);
                    return;
                }

                lock (_locker_USBTable)
                {
                    if (Filter_USBTable.Count <= 0)
                    {
                        USBLogger.Error("Filter_USBTable.Count <= 0 .");
                    }

                    foreach (RuleUSB f in Filter_USBTable)
                    {
                        if (f.IsMatchNotifyUSB(notifyUsb))
                        {
                            Match_In_FilterUSBTable(notifyUsb);
                        }
                        else
                        {
                            NotMatch_In_FilterUSBTable(notifyUsb);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                USBLogger.Log("=== "+ex.Message+" ===");
                USBLogger.Log(notifyUsb.ToString());               
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
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public Set_Filter_USBTable()
        public void Set_Filter_USBTable()
        {
            var file = USBConfig.FilterUSBTablePath;
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
                        var vid = UInt16.Parse(line.Split(',')[0]);
                        var pid = UInt16.Parse(line.Split(',')[1]);
                        var serial = line.Split(',')[2];

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
                Filter_USBTable = table;
            }
        }
        #endregion

        #region + public void UpdateUSBList_Timer()
        public void UpdateUSBList_Timer()
        {

        }
        #endregion


        //* Filter Rule *//

        #region + private void NotFound_UsbDeviceID_By_DiskPath_SetupDi(NotifyUSB usb)
        private void NotFound_UsbDeviceID_By_DiskPath_SetupDi(NotifyUSB usb)
        {

        }
        #endregion

        #region + private void NotFound_NotityUSB_VidPidSerial_In_UsbBus(NotifyUSB notifyUsb)
        private void NotFound_NotityUSB_VidPidSerial_In_UsbBus(NotifyUSB notifyUsb)
        {

        }
        #endregion

        #region + private void Match_In_FilterUSBTable(NotifyUSB usb)
        private void Match_In_FilterUSBTable(NotifyUSB usb)
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

        #region + private void NotMatch_In_FilterUSBTable(NotifyUSB usb)
        private void NotMatch_In_FilterUSBTable(NotifyUSB usb)
        {
            try
            {
                Set_Disk_IsReadOnly_by_DiskPath_WMI(usb.DiskPath, true);
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
