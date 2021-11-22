using SqlSugar;
using System;
using USBCommon;

namespace USBModel
{
    public class Tbl_UserUsbHistory : IUserUsbHistoryHttp
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public uint Id { get; set; }

        public string UsbIdentity { get; set; }

        public string ComputerIdentity { get; set; }

        public DateTime PluginTime { get; set; }
    }
}
