using System;
using System.Windows.Forms;

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
            new RuleFilter().Set_Filter_USBTable();
            _notifyService.Start_Notifier();
        }

        public void Close()
        {
            _notifyService.Close_Notifier();

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
