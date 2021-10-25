using System;

namespace USBNetLib
{
    internal class PolicyUSB
    {
        public UInt16 Vid { get; set; }

        public string Vid_Hex => "VID_" + Vid.ToString("X").PadLeft(4, '0');

        public UInt16 Pid { get; set; }

        public string Pid_Hex => "PID_" + Pid.ToString("X").PadLeft(4, '0');

        public string SerialNumber { get; set; }

        public bool IsMatchNotifyUSB(NotifyUSB usb)
        {
            return Vid == usb.Vid && Pid == usb.Pid && SerialNumber == usb.SerialNumber;
        }
    }
}
