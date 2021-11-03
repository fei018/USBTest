using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlSugar;
using USBCommon;

namespace USBDB
{
    public class ComputerInfo : IComputerInfo
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [SugarColumn(ColumnDataType = "varchar(20)")]
        public string HostName { get; private set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "varchar(50)")]
        public string Domain { get; private set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "varchar(50)")]
        public string BiosSerial { get; private set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "varchar(12)")]
        public string IPAddress { get; private set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "varchar(20)")]
        public string MACAddress { get; private set; }
    }
}
