namespace SetupClient
{
    public class Setupini
    {
        public bool UsbFilterEnabled { get; set; }
        public string UsbNotifyService { get; set; }
        public string UsbNotifyAgent { get; set; }
        public string UsbNotifyAgentDesktop { get; set; }
        public string UsbUpdateAgent { get; set; }
        public uint UpdateTimer { get; set; }
        public string UsbFilterDbUrl { get; set; }
        public string UsbRegisterUrl { get; set; }
        public string PostComputerInfoUrl { get; set; }
        public string PostComUsbHistoryInfoUrl { get; set; }
    }
}
