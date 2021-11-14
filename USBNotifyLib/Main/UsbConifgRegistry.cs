using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace USBNotifyLib.Main
{
    public class UsbConifgRegistry
    {


        public void Initial()
        {
            try
            {
                using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    using (var usbKey = hklm.CreateSubKey("SOFTWARE\\Hiphing\\USBNotify",true))
                    {
                        usbKey.SetValue("UsbFilterDbUrl", "http://localhost:5000/usb/UsbFilterDb", RegistryValueKind.String);
                        usbKey.SetValue("PostComUsbHistoryInfoUrl", "http://localhost:5000/usb/PostComputerUsbHistoryInfo", RegistryValueKind.String);
                        usbKey.SetValue("PostComputerInfoUrl", "http://localhost:5000/usb/PostComputerInfo", RegistryValueKind.String);
                        usbKey.SetValue("UpdateTimer", "", RegistryValueKind.String);
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
