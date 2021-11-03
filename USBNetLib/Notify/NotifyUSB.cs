using System;
using USBCommon;

namespace USBNetLib
{
    public class NotifyUSB : IUsbInfo
    {
        public UInt16 Vid { get; set; }

        public string Vid_Hex => "Vid_" + Vid.ToString("X").PadLeft(4, '0');

        public UInt16 Pid { get; set; }

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

        public string ToPolicyString()
        {
            return Vid.ToString() + Pid.ToString() + SerialNumber;
        }

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

        #region remark
        //public bool HasVidPidSerial()
        //{
        //    if (Vid != 0 && Pid != 0 && !string.IsNullOrEmpty(SerialNumber))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        #endregion
    }
}
