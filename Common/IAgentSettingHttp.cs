namespace USBCommon
{
    public interface IAgentSettingHttp
    {
        /// <summary>
        /// unit: minute
        /// </summary>
        int AgentTimerMinute { get; set; }

        string Version { get; set; }
    }
}
