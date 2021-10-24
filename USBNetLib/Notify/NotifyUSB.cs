using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace USBNetLib
{
    internal class NotifyUSB
    {
        public UInt16 Vid { get; set; }

        public string Vid_Hex => "Vid_" + Vid.ToString("X");

        public UInt16 Pid { get; set; }

        public string Pid_Hex => "Pid_" + Pid.ToString("X");

        public string ParentDeviceID { get; set; }

        public string DevicePath { get; set; }

        public string SerialNumber { get; set; }

        public bool HasUSBParent
        {
            get
            {
                if (!string.IsNullOrEmpty(ParentDeviceID) && ParentDeviceID.StartsWith("USB\\", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                return false;
            }
        }

        public bool HasVidPidSerial
        {
            get
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
}
