using USBCommon;

namespace USBNotifyLib
{
    public class AgentSetting : IAgentSettingHttp
    {
        public int AgentTimerMinute { get; set; }

        public string Version { get; set; }
    }
}
