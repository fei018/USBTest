using SqlSugar;
using System;
using USBCommon;

namespace USBModel
{
    public class UsbHistory : IUsbHistory
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public uint Id { get; set; }

        [SugarColumn(ColumnDataType = "varchar(100)")]
        public string UsbIdentity { get; set; }

        [SugarColumn(ColumnDataType = "varchar(100)")]
        public string ComputerIdentity { get; set; }

        public DateTime PluginTime { get; set; }
    }
}
