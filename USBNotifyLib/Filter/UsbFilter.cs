using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Collections.Concurrent;
using System.IO;

namespace USBNotifyLib
{
    public partial class UsbFilter
    {
        private readonly UsbBusController _usbBus;

        private static bool? _usbFilterEnabled;
        public static bool IsEnable
        {
            get => _usbFilterEnabled ?? UsbRegistry.UsbFilterEnabled;
            set => _usbFilterEnabled = value;
        }

        public UsbFilter()
        {
            _usbBus = new UsbBusController();
        }

        // Filter

        #region + public void Filter_NotifyUsb_Use_DriveLetter(char driveLetter)
        public void Filter_NotifyUsb_Use_DriveLetter(char driveLetter)
        {
            try
            {
                var usb = Get_NotityUsb_DiskPath_by_DriveLetter_WMI(driveLetter);
                if (usb != null)
                {
                    Filter_NotifyUsb_Use_DiskPath(usb);
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

        #region + public viod Filter_NotifyUsb_Use_DiskPath(NotifyUSB notifyUsb)
        /// <summary>
        /// 只需 notifyUsb.DiskPath 賦值
        /// </summary>
        /// <param name="notifyUsb"></param>
        public void Filter_NotifyUsb_Use_DiskPath(NotifyUsb notifyUsb)
        {
            try
            {
                if (!Find_UsbDeviceId_By_DiskPath_SetupDi(notifyUsb))
                {
                    Rule_NotFound_UsbDeviceID_By_DiskPath_SetupDi(notifyUsb);
                    return;
                }

                if (!_usbBus.Find_NotifyUSB_Detail_In_UsbBus(notifyUsb))
                {
                    Rule_NotFound_NotifyUsb_Detail_In_UsbBus(notifyUsb);
                    return;
                }

                if (UsbFilterDbHelp.IsFind(notifyUsb))
                {
                    Rule_Match_In_UsbFilterDb(notifyUsb);
                    return;
                }
                else
                {
                    Rule_NotMatch_In_UsbFilterDb(notifyUsb);
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
                var usbList = Get_All_NotifyUsb_DiskPath_List_by_BusType_USB_WMI();
                if (usbList.Count > 0)
                {
                    foreach (var usb in usbList)
                    {
                        Filter_NotifyUsb_Use_DiskPath(usb);
                    }
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

        //Find

        #region + public NotifyUSB Find_NotifyUsb_Use_DriveLetter(char driveLetter)
        public NotifyUsb Find_NotifyUsb_Use_DriveLetter(char driveLetter)
        {
            try
            {
                var usb = Get_NotityUsb_DiskPath_by_DriveLetter_WMI(driveLetter);
                if (usb != null)
                {
                    if (Find_UsbDeviceId_By_DiskPath_SetupDi(usb))
                    {
                        if (_usbBus.Find_NotifyUSB_Detail_In_UsbBus(usb))
                        {
                            return usb;
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public NotifyUsb Find_NotifyUsb_Use_DiskPath(string diskPath)
        public NotifyUsb Find_NotifyUsb_Use_DiskPath(string diskPath)
        {
            try
            {
                var usb = new NotifyUsb { DiskPath = diskPath };
                if (Find_UsbDeviceId_By_DiskPath_SetupDi(usb))
                {
                    if (_usbBus.Find_NotifyUSB_Detail_In_UsbBus(usb))
                    {
                        return usb;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        //* Filter Rule *//

        #region + private void Rule_NotFound_UsbDeviceID_By_DiskPath_SetupDi(NotifyUSB usb)
        /// <summary>
        /// 找不到 大多數係 非 USB device
        /// </summary>
        /// <param name="usb"></param>
        private void Rule_NotFound_UsbDeviceID_By_DiskPath_SetupDi(NotifyUsb usb)
        {
            UsbLogger.Log("Not Found In SetupDi Usb DeviceId: " + usb.ToString());
        }
        #endregion

        #region + private void Rule_NotFound_NotifyUsb_Detail_In_UsbBus(NotifyUsb notifyUsb)
        /// <summary>
        /// should not happen
        /// </summary>
        /// <param name="notifyUsb"></param>
        private void Rule_NotFound_NotifyUsb_Detail_In_UsbBus(NotifyUsb notifyUsb)
        {
            // should not happen
        }
        #endregion

        #region + private void Rule_Match_In_UsbFilterDb(NotifyUsb usb)
        private void Rule_Match_In_UsbFilterDb(NotifyUsb usb)
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

        #region + private void Rule_NotMatch_In_UsbFilterDb(NotifyUsb usb)
        private void Rule_NotMatch_In_UsbFilterDb(NotifyUsb usb)
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
