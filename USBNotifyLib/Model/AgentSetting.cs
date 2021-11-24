using USBCommon;

namespace USBNotifyLib
{
    public class AgentSetting : IAgentSettingHttp
    {
        public int Id { get; set; }

        public int AgentTimerMinute { get; set; }

        public string Version { get; set; }
    }
}
