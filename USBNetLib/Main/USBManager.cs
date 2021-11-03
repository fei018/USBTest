using System;
using System.Windows.Forms;
using USBNetLib.Win32API;

namespace USBNetLib
{
    public class USBManager
    {
        private readonly NotifyService _notifyService;

        public USBManager()
        {
            _notifyService = new NotifyService();
        }

        public void Start()
        {
            try
            {
                _notifyService.Start_Notifier();

                new PolicyUSBTable().Reload_PolicyUSBTable();

                new UsbPolicyFilter().Filter_Scan_All_USB_Disk();

            }
            catch (Exception ex)
            {

                USBLogger.Log(ex.Message);
            }
        }

        public void Close()
        {
            try
            {
                _notifyService.Close_Notifier();

                ExitWinFormEnvironment();
            }
            catch (Exception ex)
            {

                USBLogger.Log(ex.Message);
            }
            
        }

        /// <summary>
        /// 退出 winform 環境, app 重啟前 Application.Run() 無效.
        /// </summary>
        private void ExitWinFormEnvironment()
        {
            Application.Exit();
        }
    }
}
