namespace SetupClient
{
    public class Setupini
    {        
        public string UsbNotifyService { get; set; }
        public string UsbNotifyAgent { get; set; }
        public string UsbNotifyAgentDesktop { get; set; }
        public string UsbUpdateAgent { get; set; }



        public uint AgentTimerMinute { get; set; }
        public bool UsbFilterEnabled { get; set; }

        public string UsbFilterDbUrl { get; set; }
        public string AgentSettingUrl { get; set; }
        public string UsbRegisterUrl { get; set; }
        public string PostUserComputerUrl { get; set; }
        public string PostUserUsbHistoryUrl { get; set; }
    }
}
