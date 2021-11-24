namespace USBCommon
{
    public interface IAgentSettingHttp
    {
        int Id { get; set; }

        int AgentTimerMinute { get; set; }

        string Version { get; set; }
    }
}
