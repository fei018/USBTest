using System;
using System.Windows;
using USBNotifyAgentTray.USBWindow;
using USBNotifyLib;

namespace USBNotifyAgentTray
{
    public partial class App
    {
        private PipeClientTray _trayPipe;

        private void AppExitEvent(object sender, ExitEventArgs e)
        {
            RemoveTrayIcon();

            _trayPipe.Stop();
        }

        private void AppStartupEvent(object sender, StartupEventArgs e)
        {
            _trayPipe = new PipeClientTray();
            _trayPipe.Start();

            _trayPipe.CloseTrayEvent += _trayPipe_CloseTrayEvent;
            _trayPipe.TrayMessageBoxShowEvent += _trayPipe_TrayMessageBoxShowEvent;
            _trayPipe.TrayShowUsbRequestWindowEvent += _trayPipe_TrayShowUsbRequestWindowEvent;

            AddTrayIcon();
        }

        #region + private void _trayPipe_TrayShowUsbRequestWindowEvent(object sender, UsbDisk e)
        /// <summary>
        /// 顯示 UsbRequestWin
        /// </summary>
        private void _trayPipe_TrayShowUsbRequestWindowEvent(object sender, UsbDisk e)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    //Debugger.Break();
                    var usbWin = new UsbRequestWin();
                    usbWin.ShowPageUsbRequestNotify(e);
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
        private void _trayPipe_TrayMessageBoxShowEvent(object sender, string e)
        {
            MessageBox.Show(e, "USB Control");
        }
        #endregion

        #region + private void _trayPipe_CloseTrayEvent(object sender, EventArgs e)
        private void _trayPipe_CloseTrayEvent(object sender, EventArgs e)
        {
            RemoveTrayIcon();

            App.Current.Shutdown();
        }
        #endregion
    }
}
