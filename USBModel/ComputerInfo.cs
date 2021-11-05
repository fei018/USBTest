using SqlSugar;
using USBCommon;

namespace USBModel
{
    public class ComputerInfo : IComputerInfo
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [SugarColumn(ColumnDataType = "varchar(20)")]
        public string HostName { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "varchar(50)")]
        public string Domain { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "varchar(50)")]
        public string BiosSerial { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "varchar(12)")]
        public string IPAddress { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "varchar(20)")]
        public string MacAddress { get; set; }


        public override string ToString()
        {
            return "HostName: " + HostName + "\r\n" +
                   "Domain: " + Domain + "\r\n" +
                   "BiosSerial: " + BiosSerial + "\r\n" +
                   "IPAddress: " + IPAddress + "\r\n" +
                   "MacAddress: " + MacAddress + "\r\n";
        }
    }
}
