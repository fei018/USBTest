using NativeUsbLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBNetLib.Win32API;

namespace USBNetLib
{
    internal class NotifyDiskHelp
    {
        private readonly USBBusController _UsbBus;
        private readonly PolicyTableHelp _policyTableHelp;
        private readonly NotifySetup _notifySetup;

        public NotifyDiskHelp()
        {
            _UsbBus = new USBBusController();
            _policyTableHelp = new PolicyTableHelp();
            _notifySetup = new NotifySetup();
        }

        public bool DiskHandler(string devicePath, out NotifyDisk disk)
        {
            try
            {
                disk = new NotifyDisk { Path = devicePath };

                Guid guid = USetupApi.GUID_DEVINTERFACE.GUID_DEVINTERFACE_DISK;

                var notifyUsb = new NotifyUSB { NotifyDevicePath = devicePath };              

                if (_notifySetup.Use_NotifyPath_Find_USBDeviceID(guid, ref notifyUsb))
                {
                    if(_UsbBus.Find_VidPidSerial_In_UsbBus(ref notifyUsb))
                    {
                        if (_policyTableHelp.MatchPolicyTable(ref notifyUsb))
                        {
                            disk.ParentUSB = notifyUsb;
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
