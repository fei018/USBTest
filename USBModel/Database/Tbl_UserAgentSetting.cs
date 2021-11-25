﻿using SqlSugar;

namespace USBModel
{
    public class Tbl_UserAgentSetting
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [SugarColumn(UniqueGroupNameList = new string[] { "comid" })]
        public string ComputerIdentity { get; set; }

        public bool UsbFilterEnabled { get; set; }
    }
}