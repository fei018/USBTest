using System;

namespace USBCommon
{
    public interface IUsbInfo
    {
        UInt16 Vid { get; set; }

        string Vid_Hex { get; }

        UInt16 Pid { get; set; }

        string Pid_Hex { get; }

        string SerialNumber { get; set; }

        string Manufacturer { get; set; }

        string Product { get; set; }

        string DeviceDescription { get; set; }

        string UniqueVPSerial { get; }
    }
}
