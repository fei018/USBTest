namespace USBModel
{
    public class UserUsbHistoryDetail : Tbl_UserUsbHistory
    {
        public string ComputerName { get; set; }

        public string UsbPluginTime => PluginTime.ToString("yyyy-MM-dd HH:mm:ss");


        public UserUsbHistoryDetail(Tbl_UserComputer com = null)
        {
            ComputerName = com?.HostName;
        }
    }
}
