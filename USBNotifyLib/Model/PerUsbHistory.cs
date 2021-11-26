using System;
using USBCommon;

namespace USBNotifyLib
{
    public class PerUsbHistory : IPerUsbHistory
    {
        public int Vid { get; set; }
        public int Pid { get; set; }
        public string SerialNumber { get; set; }
        public string Manufacturer { get; set; }
        public string Product { get; set; }
        public string DeviceDescription { get; set; }

        public string UsbIdentity => (Vid.ToString() + Pid.ToString() + SerialNumber).ToLower();

        public string ComputerIdentity { get; set; }

        public DateTime PluginTime { get; set; }

        public string Vid_Hex => "VID_" + Vid.ToString("X").PadLeft(4, '0');

        public string Pid_Hex => "PID_" + Vid.ToString("X").PadLeft(4, '0');

    }
}
