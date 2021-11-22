using SqlSugar;
using USBCommon;

namespace USBModel
{
    public class Tbl_AgentSetting : IAgentSettingHttp
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        public int AgentTimerMinute { get; set; }

        [SugarColumn(ColumnDataType ="varchar(10)")]
        public string Version { get ; set ; }
    }
}
