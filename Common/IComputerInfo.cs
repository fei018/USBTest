﻿namespace USBCommon
{
    public interface IComputerInfo
    {
        string HostName { get; set; }

        string Domain { get; set; }

        string BiosSerial { get; set; }

        string IPAddress { get; set; }

        string MacAddress { get; set; }

        string ComputerIdentity { get; }
    }
}
