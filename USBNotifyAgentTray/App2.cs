using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace USBNotifyAgentTray
{
    public partial class App
    {
        private TrayPipe _trayPipe;

        private void AppStartupEvent(object sender, StartupEventArgs e)
        {
            _trayPipe = new TrayPipe();
            _trayPipe.CloseTrayEvent += _trayPipe_CloseTrayEvent;
            _trayPipe.Start();

            AddTrayIcon();
        }

        private void AppExitEvent(object sender, ExitEventArgs e)
        {
            RemoveTrayIcon();

            _trayPipe.Stop();
        }

        // TrayPipe

        #region + private void _trayPipe_CloseTrayEvent(object sender, EventArgs e)
        private void _trayPipe_CloseTrayEvent(object sender, EventArgs e)
        {
            RemoveTrayIcon();
            _trayPipe.PushMsg_ToService_TrayClosed();
            App.Current.Shutdown();
        }
        #endregion
    }
}
