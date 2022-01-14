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

        public static UsbDisk UsbDiskInfo { get; set; }

        #region Start()
        public void Stop()
        {
            try
            {
                if (_client != null)
                {
                    _client.Error -= pipeConnection_Error;
                    _client.ServerMessage -= ReceiveMessageFromAgentPipe;
                    _client.Stop();
                    _client = null;
                }
            }
            catch (Exception)
            {
            }        
        }

        public void Start()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(PipeName))
                {
                    UsbLogger.Error("PipeName is empty");
                    return;
                }

                Stop();

                _client = new NamedPipeClient<string>(PipeName);
                _client.AutoReconnect = true;

                _client.ServerMessage += ReceiveMessageFromAgentPipe;

                _client.Error += pipeConnection_Error;

                _client.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "TrayPipe Start()");
            }
        }

        private void pipeConnection_Error(Exception exception)
        {
            MessageBox.Show(exception.Message,"TrapPipe Error");
        }
        #endregion

        #region + private void ReceiveMessageFromAgentPipe(NamedPipeConnection<string, string> connection, string usbJson)
        private void ReceiveMessageFromAgentPipe(NamedPipeConnection<string, string> connection, string message)
        {
            try
            {
                var pipeMsg = JsonConvert.DeserializeObject<PipeMsg>(message);

                if (pipeMsg == null)
                {
                    throw new Exception("TrayPipe: PipeMsg is Null.");
                }

                switch (pipeMsg.PipeMsgType)
                {
                    case PipeMsgType.Error:
                        MessageFromAgentPipe(pipeMsg.Message);
                        break;

                    case PipeMsgType.Message:
                        MessageFromAgentPipe(pipeMsg.Message);
                        break;

                    case PipeMsgType.UsbDisk:
                        OpenTrayNotifyWindow(pipeMsg.UsbDisk);
                        break;

                    case PipeMsgType.CloseAgentTray:
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "TrayPipe Error");
            }          
        }
        #endregion

        // ReceiveMessageFromAgentPipe handler

        #region + private void MessageFromAgentPipe(string message)
        private void MessageFromAgentPipe(string message)
        {
            MessageBox.Show(message, "USB Control");
        }
        #endregion

        #region + private void OpenTrayNotifyWindow(UsbDisk usbDisk)
        private void OpenTrayNotifyWindow(UsbDisk usbDisk)
        {
            try
            {
                UsbDiskInfo = usbDisk;
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    //Debugger.Break();
                    var notifyWin = new NotifyWindow();

                    notifyWin.Show();
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
        #endregion

        // push message to agent pipe server

        #region + public void CheckAndUpdateAgent()
        public void CheckAndUpdateAgent()
        {           
            try
            {
                var json = JsonConvert.SerializeObject(new PipeMsg(PipeMsgType.UpdateAgent));
                _client?.PushMessage(json);
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region + public void UpdateUsbWhitelist()
        public void UpdateUsbWhitelist()
        {
            try
            {
                var json = JsonConvert.SerializeObject(new PipeMsg(PipeMsgType.UpdateUsbWhitelist));
                _client?.PushMessage(json);
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region + public void UpdateAgentSetting()
        public void UpdateAgentSetting()
        {
            try
            {
                var json = JsonConvert.SerializeObject(new PipeMsg(PipeMsgType.UpdateAgentSetting));
                _client?.PushMessage(json);
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region + public void UsbFullScan()
        public void UsbFullScan()
        {
            try
            {
                var json = JsonConvert.SerializeObject(new PipeMsg(PipeMsgType.UsbFullScan));
                _client?.PushMessage(json);
            }
            catch (Exception)
            {
            }
        }
        #endregion
    }
}
