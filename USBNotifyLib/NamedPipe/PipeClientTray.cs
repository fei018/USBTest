using NamedPipeWrapper;
using Newtonsoft.Json;
using System;

namespace USBNotifyLib
{
    public class PipeClientTray
    {
        // private
        private static string PipeName = AgentRegistry.AgentKey;

        private NamedPipeClient<string> _client;

        #region Event
        public event EventHandler CloseTrayEvent;

        public event EventHandler<string> TrayMessageBoxShowEvent;

        public event EventHandler<UsbDisk> TrayShowUsbRequestWindowEvent;
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
                    UsbLogger.Error("PipeName is empty");
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
                var pipeMsg = JsonConvert.DeserializeObject<PipeMsg>(message);

                if (pipeMsg == null)
                {
                    throw new Exception("TrayPipe: PipeMsg is Null.");
                }

                switch (pipeMsg.PipeMsgType)
                {
                    case PipeMsgType.Error:
                        Handler_FromPipeServerAgent_Msg(pipeMsg.Message);
                        break;

                    case PipeMsgType.Message:
                        Handler_FromPipeServerAgent_Msg(pipeMsg.Message);
                        break;

                    case PipeMsgType.UsbDiskNotInWhitelist:
                        Handler_FromPipeServerAgent_OpenTrayUsbNotifyWindow(pipeMsg.UsbDisk);
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
                TrayMessageBoxShowEvent?.Invoke(null, ex.Message);
            }
        }
        #endregion

        // Receive Msg From PipeServerAgent Handler

        #region + private void Handler_FromPipeServerAgent_Msg(string message)
        private void Handler_FromPipeServerAgent_Msg(string message)
        {
            TrayMessageBoxShowEvent?.Invoke(null, message);
        }
        #endregion

        #region + private void Handler_FromPipeServerAgent_OpenTrayUsbNotifyWindow(UsbDisk usbDisk)
        private void Handler_FromPipeServerAgent_OpenTrayUsbNotifyWindow(UsbDisk usbDisk)
        {
            try
            {
                TrayShowUsbRequestWindowEvent?.Invoke(null, usbDisk);
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region + private void Handler_CloseTray()
        private void Handler_CloseTray()
        {
            try
            {
                CloseTrayEvent?.Invoke(null, null);
            }
            catch (Exception)
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
    }
}
