namespace USBCommon
{
    public interface IComputerInfo
    {
        string HostName { get; }

        string Domain { get; }

        string BiosSerial { get; }

        string IPAddress { get; }

        string MacAddress { get; }
    }
}
