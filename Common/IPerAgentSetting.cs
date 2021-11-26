namespace USBCommon
{
    public interface IPerAgentSetting
    {
        string ComputerIdentity { get; set; }

        bool UsbFilterEnabled { get; set; }

        bool UsbHistoryEnabled { get; set; }
    }
}
