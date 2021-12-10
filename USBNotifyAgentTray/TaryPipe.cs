using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBNotifyLib;
using NamedPipeWrapper;
using System.Diagnostics;
using System.Windows.Threading;

namespace USBNotifyAgentTray
{
    public class TrayPipe
    {
        private const string PipeName = "USB-b50ae7e9-5f14-4874-a273-9e119e8791e1";

        private NamedPipeClient<NotifyUsb> _client;

        public void Start()
        {
            _client?.Stop();

            _client = new NamedPipeClient<NotifyUsb>(PipeName);
            _client.AutoReconnect = true;

            _client.ServerMessage += _client_ServerMessage;

            _client.Start();
            _client.WaitForConnection();

        }

        private void _client_ServerMessage(NamedPipeConnection<NotifyUsb, NotifyUsb> connection, NotifyUsb usb)
        {

            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Debugger.Break();
                var notifyWin = new NotifyWindow();
                notifyWin.NotifyUsb = usb;
                notifyWin.Show();
            }));
        }

        public void Stop()
        {
            _client?.Stop();
        }
    }
}
