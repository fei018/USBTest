using System;
using System.Collections.Generic;
using System.Text;

namespace USBModel
{
    public class UserUsbHistoryDetail : UserUsbHistory
    {
        public int Vid { get; set; }

        public int Pid { get; set; }

        public string SerialNumber { get; set; }

        public string Manufacturer { get; set; }

        public string Product { get; set; }

        public string ComputerName { get; set; }

        public string UsbPluginTime => PluginTime.ToString("yyyy-MM-dd HH:mm:ss");


        public UserUsbHistoryDetail(UserUsbHistory usbHistory, UsbInfo usb, UserComputer com=null)
        {
            Vid = usb.Vid;
            Pid = usb.Pid;
            SerialNumber = usb.SerialNumber;
            Manufacturer = usb.Manufacturer;
            Product = usb.Product;           
            UsbIdentity = usbHistory.UsbIdentity;
            ComputerIdentity = usbHistory.ComputerIdentity;
            PluginTime = usbHistory.PluginTime;

            ComputerName = com?.HostName;
        }
    }
}
