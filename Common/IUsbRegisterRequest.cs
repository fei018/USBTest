using System;

namespace USBCommon
{
    public interface IUsbRegRequest : IUsbInfo
    {
        string UserEmail { get; set; }

        string RequestComputerIdentity { get; set; }

        DateTime RequestTime { get; set; }       
    }
}
