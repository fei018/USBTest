namespace USBNotifyLib
{
    public class AgentManager
    {
        #region + public static void Startup()
        public static void Startup()
        {
            // 上載 本機資訊
            new AgentHttpHelp().PostPerComputer_Http();

            // registry 讀取 UsbFilterEnable 設定
            if (AgentRegistry.UsbFilterEnabled)
            {
                // 載入 UsbFilterDb cache
                UsbFilterDataHelp.Reload_UsbFilterData();

                // 掃描所有 usb disk
                new UsbFilter().Filter_Scan_All_USB_Disk();
            }

            // 執行 agent 定時任務
            UsbTimer.RunTask();
        }
        #endregion
    }
}
