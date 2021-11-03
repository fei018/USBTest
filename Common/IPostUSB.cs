namespace USBCommon
{
    public interface IPostUSB
    {
        IComputerInfo ComputerInfo { get; set; }

        IUsbInfo UsbInfo { get; set; }
    }
}
