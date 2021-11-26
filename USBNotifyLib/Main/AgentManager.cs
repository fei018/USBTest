namespace USBNotifyLib
{
    public class AgentManager
    {
        public static void Startup()
        {
            new UsbHttpHelp().PostPerComputer_Http();

            if (UsbFilter.IsEnable)
            {
                // 載入 UsbFilterDb cache
                UsbFilterDbHelp.Reload_UsbFilterDb();

                // 掃描所有 usb disk
                new UsbFilter().Filter_Scan_All_USB_Disk();
            }

            // 執行 agent 定時任務
            UsbTimer.RunTask();
        }
    }
}
