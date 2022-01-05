using System;
using USBCommon;

namespace USBNotifyLib
{
    public class UsbRegRequest : IUsbRegRequest
    {
        public int Vid { get; set; }

        public string Vid_Hex => "Vid_" + Vid.ToString("X").PadLeft(4, '0');

        public int Pid { get; set; }

        public string Pid_Hex => "Pid_" + Pid.ToString("X").PadLeft(4, '0');

        public string SerialNumber { get; set; }

        public string Manufacturer { get; set; }

        public string Product { get; set; }

        public string DeviceDescription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Lowercase string</returns>
        public string UsbIdentity => (Vid.ToString() + Pid.ToString() + SerialNumber).ToLower();

        public string UserEmail { get; set; }

        public string RequestComputerIdentity { get; set; }

        public DateTime RequestTime { get; set; }

        public UsbRegRequest(IUsbInfo usb, string userEmail=null)
        {
            Vid = usb.Vid;
            Pid = usb.Pid;
            SerialNumber = usb.SerialNumber;
            Manufacturer = usb.Manufacturer;
            Product = usb.Product;
            DeviceDescription = usb.DeviceDescription;

            UserEmail = userEmail;
            RequestComputerIdentity = PerComputerHelp.GetComputerIdentity();
            RequestTime = DateTime.Now;
        }
    }
}
