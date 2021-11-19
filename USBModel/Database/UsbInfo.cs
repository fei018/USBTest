using SqlSugar;
using System;

namespace USBModel
{
    public class UsbInfo
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [SugarColumn(UniqueGroupNameList = new string[] { "usbid" })]
        public string UsbIdentity { get; set; }

        public bool IsRegistered { get; set; }

        public int Vid { get; set; }

        public int Pid { get; set; }

        public string SerialNumber { get; set; }


        [SugarColumn(IsNullable = true)]
        public string Manufacturer { get; set; }

        [SugarColumn(IsNullable = true)]
        public string DeviceDescription { get; set; }

        [SugarColumn(IsNullable = true)]
        public string Product { get; set; }          
    }
}
