using System;
using System.Collections.Generic;
using System.Text;

namespace USBModel
{
    public class UserUsbHistoryDetail : Tbl_UserUsbHistory
    {
        public int Vid { get; set; }

        public int Pid { get; set; }

        public string SerialNumber { get; set; }

        public string Manufacturer { get; set; }

        public string Product { get; set; }

        public string ComputerName { get; set; }

        public string UsbPluginTime => PluginTime.ToString("yyyy-MM-dd HH:mm:ss");


        public UserUsbHistoryDetail(Tbl_UserUsbHistory usbHistory, Tbl_UsbInfo usb, Tbl_UserComputer com=null)
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
