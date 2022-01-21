using System;
using System.Windows;
using USBNotifyAgentTray.USBWindow;
using USBNotifyLib;
using USBNotifyAgentTray.PrintWindow;
using System.Diagnostics;

namespace USBNotifyAgentTray
{
    public partial class App
    {
        private const string WinTitle = "IT Support Tools";

        public static PipeClientTray TrayPipe;

        private void AppExitEvent(object sender, ExitEventArgs e)
        {
#if DEBUG
            Debugger.Break();
#endif
            RemoveTrayIcon();
            TrayPipe.Stop();
        }

        private void AppStartupEvent(object sender, StartupEventArgs e)
        {
            TrayPipe = new PipeClientTray();
            TrayPipe.Start();

            TrayPipe.CloseTrayEvent += _trayPipe_CloseTrayEvent;
            TrayPipe.TrayMessageBoxShowEvent += _trayPipe_TrayMessageBoxShowEvent;
            TrayPipe.TrayShowUsbRequestWindowEvent += _trayPipe_TrayShowUsbRequestWindowEvent;

            AddTrayIcon();
        }

        #region + private void _trayPipe_TrayShowUsbRequestWindowEvent(object sender, PipeEventArgs e)
        /// <summary>
        /// 顯示 UsbRequestWin
        /// </summary>
        private void _trayPipe_TrayShowUsbRequestWindowEvent(object sender, PipeEventArgs e)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    //Debugger.Break();
                    var usbWin = new UsbRequestWin();
                    usbWin.ShowPageUsbRequestNotify(e.UsbDiskInfo);
                    usbWin.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "USB Control");
                }
            }));
        }
        #endregion

        #region + private void _trayPipe_TrayMessageBoxShowEvent(object sender, string e)
        private void _trayPipe_TrayMessageBoxShowEvent(object sender, PipeEventArgs e)
        {
            MessageBox.Show(e.Msg, WinTitle);
        }
        #endregion

        #region + private void _trayPipe_CloseTrayEvent(object sender, EventArgs e)
        private void _trayPipe_CloseTrayEvent(object sender, EventArgs e)
        {
#if DEBUG
            Debugger.Break();
#endif

            RemoveTrayIcon();

            App.Current.Shutdown();
        }
        #endregion
    }
}
