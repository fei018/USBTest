namespace USBModel
{
    public class UserUsbHistoryDetail : Tbl_PerUsbHistory
    {
        public string ComputerName { get; set; }


        public UserUsbHistoryDetail(Tbl_PerComputer com = null)
        {
            ComputerName = com?.HostName;
        }
    }
}
