using System;
using System.Windows.Forms;

namespace USBNetLib
{
    public class USBManager
    {
        private readonly NotifyService _notifyHelper;

        public USBManager()
        {
            _notifyHelper = new NotifyService();
        }

        public void Start()
        {
            new PolicyTableHelp().SetPolicyUSBList();
            _notifyHelper.Start_Notifier();
        }

        public void Close()
        {
            _notifyHelper.Close_Notifier();

            ExitWinFormEnvironment();
            
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
