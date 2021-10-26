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

        public string DeviceId { get; set; }

        public string Path { get; set; }

        public string DiskDeviceId { get; set; }

        public string DiskPath { get; set; }

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

        public override string ToString()
        {
            string s = "*** NotifyUSB ***" + Environment.NewLine +
                       Vid_Hex + Environment.NewLine +
                       Pid_Hex + Environment.NewLine +
                       "SerialNumber: " + SerialNumber + Environment.NewLine +
                       "DeviceId: " + DeviceId + Environment.NewLine +
                       "Device Path: " + Path + Environment.NewLine +
                       "Disk DeviceId: " + DiskDeviceId + Environment.NewLine +
                       "Disk Path: " + DiskPath + Environment.NewLine;
            return s;
        }
    }
}
