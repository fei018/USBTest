using System;
using SqlSugar;
using USBCommon;

namespace USBDB
{
    public class UsbInfo : IUsbInfo
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        public UInt16 Vid { get; set; }       

        public UInt16 Pid { get; set; }

        [SugarColumn(ColumnDataType = "varchar(100)")]
        public string SerialNumber { get; set; }

        [SugarColumn(IsNullable =true, ColumnDataType = "varchar(100)")]
        public string Manufacturer { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "varchar(100)")]
        public string DeviceDescription { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "varchar(100)")]
        public string Product { get; set; }



        [SugarColumn(IsIgnore =true)]
        public string Pid_Hex => "PID_" + Pid.ToString("X").PadLeft(4, '0');
        [SugarColumn(IsIgnore = true)]
        public string Vid_Hex => "VID_" + Vid.ToString("X").PadLeft(4, '0');
    }
}
