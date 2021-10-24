using System;

namespace USBNetLib
{
    internal class PolicyUSB
    {
        public UInt16 VID { get; set; }

        public string VID_Hex => "VID_" + VID.ToString("X");

        public UInt16 PID { get; set; }

        public string PID_Hex => "PID_" + PID.ToString("X");

        public string SerialNumber { get; set; }

    }
}
