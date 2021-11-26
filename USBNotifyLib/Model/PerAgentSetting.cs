using USBCommon;

namespace USBNotifyLib
{
    public class PerAgentSetting : IPerAgentSetting
    {
        public string ComputerIdentity { get; set; }

        public bool UsbFilterEnabled { get; set; }

        public bool UsbHistoryEnabled { get; set; }
    }
}
