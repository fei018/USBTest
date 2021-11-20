using System;
using USBCommon;

namespace USBNotifyLib
{
    public class NotifyUsb : IUsbInfoHttp
    {
        public int Vid { get; set; }

        public string Vid_Hex => "Vid_" + Vid.ToString("X").PadLeft(4, '0');

        public int Pid { get; set; }

        public string Pid_Hex => "Pid_" + Pid.ToString("X").PadLeft(4, '0');

        public string SerialNumber { get; set; }

        public string Manufacturer { get; set; }

        public string Product { get; set; }

        public string DeviceDescription { get; set; }

        public string DeviceId { get; set; }

        public string Path { get; set; }

        public string DiskDeviceId { get; set; }

        public string DiskPath { get; set; }

        public uint DiskNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Lowercase string</returns>
        public string UsbIdentity => (Vid.ToString() + Pid.ToString() + SerialNumber).ToLower();

        public override string ToString()
        {
            string s = "*** USB Details ***" + Environment.NewLine +
                       Vid_Hex + " - " + Vid + Environment.NewLine +
                       Pid_Hex + " - " + Pid + Environment.NewLine +
                       "SerialNumber: " + SerialNumber + Environment.NewLine +
                       "Manufacturer: " + Manufacturer + Environment.NewLine +
                       "Product: " + Product + Environment.NewLine +
                       "DeviceDescription: " + Environment.NewLine +
                       "DeviceId: " + DeviceId + Environment.NewLine +
                       "Device Path: " + Path + Environment.NewLine + Environment.NewLine;

            return s;
        }

    }
}
