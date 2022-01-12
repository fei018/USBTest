﻿using System;
using USBCommon;

namespace USBNotifyLib
{
    public class UsbRequest : UsbBase, IUsbRequest
    {
        public string RequestUserEmail { get; set; }

        public string RequestComputerIdentity { get; set; }

        public DateTime RequestTime { get; set; }

        public string RequestReason { get; set; }

        public UsbRequest(IUsbInfo usb, string userEmail = null, string reason = null)
        {
            Vid = usb.Vid;
            Pid = usb.Pid;
            SerialNumber = usb.SerialNumber;
            Manufacturer = usb.Manufacturer;
            Product = usb.Product;
            DeviceDescription = usb.DeviceDescription;

            RequestUserEmail = userEmail;
            RequestComputerIdentity = PerComputerHelp.GetComputerIdentity();
            RequestTime = DateTime.Now;
            RequestReason = reason;
        }
    }
}
