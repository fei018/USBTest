namespace USBCommon
{
    public class PostUserUsbHistoryHttp
    {
        public IUserComputerHttp UserComputer { get; set; }

        public IUsbInfoHttp UsbInfo { get; set; }

        public IUserUsbHistoryHttp UserUsbHistory { get; set; }
    }
}
