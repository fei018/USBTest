namespace USBCommon
{
    public class PostComUsbHistoryHttp
    {
        public IComputerHttp ComputerInfo { get; set; }

        public IUsbHttp UsbInfo { get; set; }

        public IUsbHistoryHttp UsbHistory { get; set; }
    }
}
