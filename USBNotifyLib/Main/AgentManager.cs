namespace USBNotifyLib
{
    public class AgentManager
    {
        #region IsUsbFilterEnable
        private static bool? _usbFilterEnabled;
        public static bool IsUsbFilterEnable
        {
            get => _usbFilterEnabled ?? UsbRegistry.UsbFilterEnabled;
            set => _usbFilterEnabled = value;
        }
        #endregion

        #region IsUsbHistoryEnable
        private static bool? _usbHistoryEnabled;
        public static bool IsUsbHistoryEnable
        {
            get => _usbHistoryEnabled ?? UsbRegistry.UsbHistoryEnabled;
            set => _usbHistoryEnabled = value;
        }
        #endregion

        #region + public static void Startup()
        public static void Startup()
        {
            // 上載 本機資訊
            new UsbHttpHelp().PostPerComputer_Http();

            // registry 讀取 UsbFilterEnable 設定
            if (IsUsbFilterEnable)
            {
                // 載入 UsbFilterDb cache
                UsbFilterDbHelp.Reload_UsbFilterDb();

                // 掃描所有 usb disk
                new UsbFilter().Filter_Scan_All_USB_Disk();
            }

            // 執行 agent 定時任務
            UsbTimer.RunTask();
        }
        #endregion

    }
}
