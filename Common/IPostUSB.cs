namespace USBCommon
{
    public interface IPostUsb
    {
        IComputerInfo ComputerInfo { get; set; }

        IUsbInfo UsbInfo { get; set; }
    }
}
