namespace USBModel
{
    public class UserUsbHistoryDetail : Tbl_PerUsbHistory
    {
        public string ComputerName { get; set; }

        public string UsbPluginTime => PluginTime.ToString("yyyy-MM-dd HH:mm:ss");


        public UserUsbHistoryDetail(Tbl_PerComputer com = null)
        {
            ComputerName = com?.HostName;
        }
    }
}
