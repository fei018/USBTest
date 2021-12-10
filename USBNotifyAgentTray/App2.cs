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
        private TrayPipe _trayPipe = new TrayPipe();

        private void AppStartup(object sender, StartupEventArgs e)
        {
            AddTrayIcon();

            _trayPipe.Start();
        }

        private void AppExit(object sender, ExitEventArgs e)
        {
            RemoveTrayIcon();

            _trayPipe.Stop();
        }
    }
}
