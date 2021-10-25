using NativeUsbLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBNetLib.Win32API;

namespace USBNetLib.Notify
{
    public class NotifyDiskHelp
    {
        public NotifyUSB DiskHandler(string devicePath)
        {
            try
            {
                Guid guid = USetupApi.GUID_DEVINTERFACE.GUID_DEVINTERFACE_DISK;

                var notifyUsb = new NotifyUSB { NotifyDevicePath = devicePath };

                var setup = new NotifySetup();

                if (setup.Use_NotifyPath_Find_USBDeviceID(guid, ref notifyUsb))
                {
                    if(new NotifyUsbBus().Find_VidPidSerial_In_UsbBus(ref notifyUsb))
                    {
                        if (new PolicyTable().MatchPolicyTable(ref notifyUsb))
                        {

                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
