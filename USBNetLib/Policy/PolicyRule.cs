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
        public void DiskPath_NotFound_UsbDeviceId_SetupDi(string diskPath)
        {

        }

        public void NotityUSB_NotFound_VidPidSerial_In_UsbBus(NotifyUSB notifyUsb)
        {

        }

        #region 
        public void IsNotMatch_In_PolicyTable(NotifyUSB notifyUsb)
        {
            USBLogger.Log("Usb no match in policy table:");
            USBLogger.Log(notifyUsb.ToString());
        }
        #endregion
    }
}
