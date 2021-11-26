using SqlSugar;
using USBCommon;

namespace USBModel
{
    public class Tbl_PerAgentSetting : IPerAgentSetting
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [SugarColumn(UniqueGroupNameList = new string[] { "comid" })]
        public string ComputerIdentity { get; set; }

        public bool UsbFilterEnabled { get; set; }

        public bool UsbHistoryEnabled { get; set; }
    }
}
