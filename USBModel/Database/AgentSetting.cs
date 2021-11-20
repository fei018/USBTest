using SqlSugar;

namespace USBModel
{
    public class AgentSetting
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [SugarColumn(DefaultValue = "10")]
        public int AgentTimerMinute { get; set; }
    }
}
