using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBNotifyLib;
using NamedPipeWrapper;
using System.Diagnostics;
using System.Windows.Threading;
using Newtonsoft.Json;
using System.Windows;

namespace USBNotifyAgentTray
{
    public class TrayPipe
    {
        private static string PipeName = AgentRegistry.AgentKey;

        private NamedPipeClient<string> _client;

        public static UsbDisk UsbDiskMessage { get; set; }

        #region
        public void Stop()
        {
            _client?.Stop();
        }

        public void Start()
        {
            if (string.IsNullOrWhiteSpace(PipeName))
            {
                UsbLogger.Error("PipeName is empty");
                return;
            }

            _client?.Stop();

            _client = new NamedPipeClient<string>(PipeName);
            _client.AutoReconnect = true;

            _client.ServerMessage += _client_ServerMessage;

            _client.Error += _client_Error;

            _client.Start();

        }

        private void _client_Error(Exception exception)
        {
            MessageBox.Show(exception.Message,"TrapPipe Error");
        }
        #endregion

        #region + private void _client_ServerMessage(NamedPipeConnection<string, string> connection, string usbJson)
        private void _client_ServerMessage(NamedPipeConnection<string, string> connection, string usbJson)
        {
            if (string.IsNullOrEmpty(usbJson))
            {
                UsbLogger.Error("TrayPipe: UsbDisk is Null from Pipe Message.");
                return;
            }

            var usb = JsonConvert.DeserializeObject<UsbDisk>(usbJson);

            if (usb == null)
            {
                throw new Exception("UsbRegisterRequest is Null.");
            }

            UsbDiskMessage = usb;
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                //Debugger.Break();
                var notifyWin = new NotifyWindow();

                notifyWin.Show();
            }));
        }
        #endregion

        #region + public void CheckAndUpdateAgent()
        public void CheckAndUpdateAgent()
        {           
            try
            {
                if(new AgentUpdate().IsNeedToUpdate())
                {
                    _client?.PushMessage("UpdateAgent");
                }
                else
                {
                    MessageBox.Show("Agent is Newest version.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

    }
}
