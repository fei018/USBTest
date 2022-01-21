using NamedPipeWrapper;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace USBNotifyLib
{
    public class PipeClientTray
    {
        // private
        private static string PipeName = AgentRegistry.AgentHttpKey;

        private NamedPipeClient<string> _client;

        #region Event
        public event EventHandler CloseTrayEvent;

        public event EventHandler<PipeEventArgs> TrayMessageBoxShowEvent;

        public event EventHandler<PipeEventArgs> TrayShowUsbRequestWindowEvent;

        public event EventHandler<PipeEventArgs> AddPrintTemplateCompletedEvent;
        #endregion


        #region Start()
        public void Stop()
        {
            try
            {
                if (_client != null)
                {
                    _client.ServerMessage -= ReceiveMsg_FromPipeServerAgent;
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
                    AgentLogger.Error("PipeName is empty");
                    return;
                }

                Stop();

                _client = new NamedPipeClient<string>(PipeName);
                _client.AutoReconnect = true;

                _client.ServerMessage += ReceiveMsg_FromPipeServerAgent;

                _client.Start();
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region + private void ReceiveMsg_FromPipeServerAgent(NamedPipeConnection<string, string> connection, string usbJson)
        private void ReceiveMsg_FromPipeServerAgent(NamedPipeConnection<string, string> connection, string message)
        {
            try
            {
                //Debugger.Break();
                var pipeMsg = JsonConvert.DeserializeObject<PipeMsg>(message);

                if (pipeMsg == null)
                {
                    throw new Exception("TrayPipe: PipeMsg is Null.");
                }

                switch (pipeMsg.PipeMsgType)
                {
                    case PipeMsgType.Message:
                        Handler_FromAgentMsg_ToTrayMessageBox(pipeMsg.Message);
                        break;

                    case PipeMsgType.UsbDiskNotInWhitelist:
                        Handler_FromAgentMsg_ToOpenTrayUsbNotifyWindow(pipeMsg.UsbDisk);
                        break;

                    case PipeMsgType.CloseTray:
                        Handler_FromAgentMsg_ToCloseTray();
                        break;

                    case PipeMsgType.AddPrintTemplateCompleted:
                        Handler_FromAgentMsg_AddPrintTemplateCompleted(pipeMsg.Message);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                TrayMessageBoxShowEvent?.Invoke(null, new PipeEventArgs(ex.Message));
            }
        }
        #endregion

        // Receive Msg From PipeServerAgent Handler

        #region + private void Handler_FromAgentMsg_ToTrayMessageBox(string message)
        private void Handler_FromAgentMsg_ToTrayMessageBox(string message)
        {
            TrayMessageBoxShowEvent?.Invoke(null, new PipeEventArgs(message));
        }
        #endregion

        #region + private void Handler_FromAgentMsg_ToOpenTrayUsbNotifyWindow(UsbDisk usbDisk)
        private void Handler_FromAgentMsg_ToOpenTrayUsbNotifyWindow(UsbDisk usbDisk)
        {
            try
            {
                TrayShowUsbRequestWindowEvent?.Invoke(null, new PipeEventArgs(usbDisk));
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region + private void Handler_FromAgentMsg_ToCloseTray()
        private void Handler_FromAgentMsg_ToCloseTray()
        {
            try
            {
                CloseTrayEvent?.Invoke(null, null);
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region Handler_FromAgentMsg_AddPrintTemplateCompleted(string msg)
        private void Handler_FromAgentMsg_AddPrintTemplateCompleted(string msg)
        {
            try
            {
                AddPrintTemplateCompletedEvent?.Invoke(null, new PipeEventArgs(msg));
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        // push message

        #region + public void PushMsg_ToAgent_CheckAndUpdateAgent()
        public void PushMsg_ToAgent_CheckAndUpdateAgent()
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

        #region + public void PushMsg_ToAgent_UpdateSetting()
        public void PushMsg_ToAgent_UpdateSetting()
        {
            try
            {
                var json = JsonConvert.SerializeObject(new PipeMsg(PipeMsgType.UpdateSetting));
                _client?.PushMessage(json);
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region + public void PushMsg_ToAgent_AddPrintTemplate()
        public void PushMsg_ToAgent_AddPrintTemplate()
        {
            try
            {
                var json = JsonConvert.SerializeObject(new PipeMsg(PipeMsgType.AddPrintTemplate));
                _client?.PushMessage(json);
            }
            catch (Exception)
            {
            }
        }
        #endregion
    }
}
