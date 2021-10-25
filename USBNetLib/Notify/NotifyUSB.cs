using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using USBNetLib.Notify;

namespace USBNetLib
{
    internal class NotifyUSB
    {
        public UInt16 Vid { get; set; }

        public string Vid_Hex => "Vid_" + Vid.ToString("X").PadLeft(4,'0');

        public UInt16 Pid { get; set; }

        public string Pid_Hex => "Pid_" + Pid.ToString("X").PadLeft(4, '0');

        public string DeviceID { get; set; }

        public string DevicePath { get; set; }

        public string NotifyDeviceID { get; set; }

        public string NotifyDevicePath { get; set; }

        public string SerialNumber { get; set; }

        public bool HasVidPidSerial()
        {
            if (Vid != 0 && Pid != 0 && !string.IsNullOrEmpty(SerialNumber))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
