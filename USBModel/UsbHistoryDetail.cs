using System;
using System.Collections.Generic;
using System.Text;

namespace USBModel
{
    public class UsbHistoryDetail : UsbHistory
    {
        public int Vid { get; set; }

        public int Pid { get; set; }

        public string SerialNumber { get; set; }

        public string Manufacturer { get; set; }

        public string Product { get; set; }

        public string Computer { get; set; }

        public string UsbPluginTime => PluginTime.ToString("yyyy-MM-dd HH:mm:ss");

        public UsbHistoryDetail(UsbHistory usbHistory, UserUsb usb, UserComputer com)
        {
            Vid = usb.Vid;
            Pid = usb.Pid;
            SerialNumber = usb.SerialNumber;
            Manufacturer = usb.Manufacturer;
            Product = usb.Product;
            Computer = com.HostName;
            UsbIdentity = usbHistory.UsbIdentity;
            ComputerIdentity = usbHistory.ComputerIdentity;
            PluginTime = usbHistory.PluginTime;
        }
    }
}
