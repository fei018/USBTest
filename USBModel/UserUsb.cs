using SqlSugar;
using System;
using USBCommon;

namespace USBModel
{
    public class UserUsb : IUsbInfo
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        public bool IsRegistered { get; set; }

        [SugarColumn(IsNullable = true)]
        public DateTime RegisteredTime { get; set; }

        public UInt16 Vid { get; set; }

        public UInt16 Pid { get; set; }

        [SugarColumn(ColumnDataType = "varchar(100)")]
        public string SerialNumber { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "varchar(100)")]
        public string Manufacturer { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "varchar(100)")]
        public string DeviceDescription { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "varchar(100)")]
        public string Product { get; set; }

        [SugarColumn(UniqueGroupNameList = new string[] {"unique1"}, ColumnDataType = "varchar(100)")]
        public string UsbIdentity { get; set; }

        [SugarColumn(ColumnDataType = "varchar(100)")]
        public string RequestComputerId { get; set; }

        [SugarColumn(IsNullable = true)]
        public DateTime RequestTime { get; set; }

        
        

        // SugarColumn(IsIgnore = true)

        [SugarColumn(IsIgnore = true)]
        public string Pid_Hex => "PID_" + Pid.ToString("X").PadLeft(4, '0');

        [SugarColumn(IsIgnore = true)]
        public string Vid_Hex => "VID_" + Vid.ToString("X").PadLeft(4, '0');


        public override string ToString()
        {
            return $"\r\nManufacturer: {Manufacturer}\r\nProduct: { Product}\r\nVid: {Vid_Hex}\r\nPid: {Pid_Hex}\r\nSerialNumber: {SerialNumber}\r\n";
        }
    }
}
