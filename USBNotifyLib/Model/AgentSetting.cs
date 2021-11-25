using USBCommon;

namespace USBNotifyLib
{
    public class AgentSetting : IAgentSetting
    {
        public int AgentTimerMinute { get; set; }

        public string Version { get; set; }
    }
}
