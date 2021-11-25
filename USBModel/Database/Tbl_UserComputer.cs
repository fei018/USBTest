using SqlSugar;
using System;
using USBCommon;

namespace USBModel
{
    public class Tbl_UserComputer : IUserComputer
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [SugarColumn(UniqueGroupNameList = new string[] { "comid" })]
        public string ComputerIdentity { get; set; }

        public string HostName { get; set; }

        [SugarColumn(IsNullable = true)]
        public string Domain { get; set; }

        public string BiosSerial { get; set; }

        [SugarColumn(IsNullable = true)]
        public string IPAddress { get; set; }

        public string MacAddress { get; set; }      

        public DateTime LastSeen { get; set; }

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
