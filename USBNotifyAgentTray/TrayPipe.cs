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

        public event EventHandler CloseTrayEvent;

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

        private void pipeConnection_Error(Exception ex)
        {
            MessageBox.Show(ex.Message,"TrapPipe Error");
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
                        Handler_MessageFromAgentPipe(pipeMsg.Message);
                        break;

                    case PipeMsgType.Message:
                        Handler_MessageFromAgentPipe(pipeMsg.Message);
                        break;

                    case PipeMsgType.UsbDisk:
                        Handler_OpenTrayNotifyWindow(pipeMsg.UsbDisk);
                        break;

                    case PipeMsgType.CloseTray:
                        Handler_CloseTray();
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

        #region + private void Handler_MessageFromAgentPipe(string message)
        private void Handler_MessageFromAgentPipe(string message)
        {
            MessageBox.Show(message, "USB Control");
        }
        #endregion

        #region + private void Handler_OpenTrayNotifyWindow(UsbDisk usbDisk)
        private void Handler_OpenTrayNotifyWindow(UsbDisk usbDisk)
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

        #region + private void Handler_CloseTray()
        private void Handler_CloseTray()
        {
            try
            {
                CloseTrayEvent?.Invoke(this, null);
            }
            catch (Exception)
            {
            }
        }
        #endregion

        // push message

        #region + public void PushMsg_ToAgent_CheckAndUpdateAgent()
        public void  PushMsg_ToAgent_CheckAndUpdateAgent()
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

        #region + public void PushMsg_ToAgent_UpdateUsbWhitelist()
        public void PushMsg_ToAgent_UpdateUsbWhitelist()
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

        #region + public void PushMsg_ToAgent_UpdateAgentSetting()
        public void PushMsg_ToAgent_UpdateAgentSetting()
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

        #region + public void PushMsg_ToAgent_UsbFullScan UsbFullScan()
        public void PushMsg_ToAgent_UsbFullScan()
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

        #region + public void PushMsg_ToService_TrayClosed()
        public void PushMsg_ToService_TrayClosed()
        {
            try
            {
                var json = JsonConvert.SerializeObject(new PipeMsg(PipeMsgType.TrayClosed));
                _client?.PushMessage(json);
            }
            catch (Exception)
            {
            }
        }
        #endregion
    }
}
