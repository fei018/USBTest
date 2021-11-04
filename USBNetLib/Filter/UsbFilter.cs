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
    public partial class UsbFilter
    {
        private readonly UsbBusController _UsbBus;

        private readonly UsbFilterTable _ruleTable;

        public UsbFilter()
        {
            _UsbBus = new UsbBusController();
            _ruleTable = new UsbFilterTable();
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
                UsbLogger.Error(ex.Message);
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

                if (_ruleTable.IsFind(notifyUsb))
                {
                    Rule_Match_In_RuleUSBTable(notifyUsb);
                    return;
                }
                else
                {
                    Rule_NotMatch_In_RuleUSBTable(notifyUsb);
                }             
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
                UsbLogger.Error(notifyUsb.ToString());               
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
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

  
        //* Filter Rule *//

        #region + private void Rule_NotFound_UsbDeviceID_By_DiskPath_SetupDi(NotifyUSB usb)
        /// <summary>
        /// 找不到 大多數係 非 USB device
        /// </summary>
        /// <param name="usb"></param>
        private void Rule_NotFound_UsbDeviceID_By_DiskPath_SetupDi(NotifyUSB usb)
        {
            UsbLogger.Log("Not Found In SetupDi Usb DeviceId: " + usb.ToString());
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

        #region + private void Rule_Match_In_RuleUSBTable(NotifyUSB usb)
        private void Rule_Match_In_RuleUSBTable(NotifyUSB usb)
        {
            try
            {
                Set_Disk_IsReadOnly_by_DiskPath_WMI(usb.DiskPath, false);
                UsbLogger.Log("=== Match In USB ===");
                UsbLogger.Log(usb.ToString());
                UsbLogger.Log("------");
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region + private void Rule_NotMatch_In_FilterUSBTable(NotifyUSB usb)
        private void Rule_NotMatch_In_RuleUSBTable(NotifyUSB usb)
        {
            try
            {
                Set_Disk_IsReadOnly_by_DiskPath_WMI(usb.DiskPath, true);
                NotMatchSendMessage(usb);

                UsbLogger.Log("=== Not Match USB ===");
                UsbLogger.Log(usb.ToString());
                UsbLogger.Log("Set Disk ReadOnly Success.");
                UsbLogger.Log("------");
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        #endregion
    }
}
