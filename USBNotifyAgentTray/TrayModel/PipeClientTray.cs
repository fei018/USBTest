using NamedPipeWrapper;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using USBNotifyLib;
using System.Windows;
using USBNotifyAgentTray.USBWindow;
using System.IO;

namespace USBNotifyAgentTray
{
    public class PipeClientTray
    {
        // private
        private static string PipeName = AgentRegistry.AgentHttpKey;

        private NamedPipeClient<string> _client;

        // public static
        public static PipeClientTray Entity {get;set;}

        #region Event
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
                throw;
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
                        Handler_FromAgentMsg_MessageBox(pipeMsg.Message);
                        break;

                    case PipeMsgType.UsbDiskNotInWhitelist:
                        Handler_FromAgentMsg_UsbNotifyWindow(pipeMsg.UsbDisk);
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
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        // Receive Msg From PipeServerAgent Handler

        #region + private void Handler_FromAgentMsg_MessageBox(string message)
        private void Handler_FromAgentMsg_MessageBox(string message)
        {
            MessageBox.Show(message);
        }
        #endregion

        #region + private void Handler_FromAgentMsg_UsbNotifyWindow(UsbDisk usbDisk)
        private void Handler_FromAgentMsg_UsbNotifyWindow(UsbDisk usbDisk)
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    //Debugger.Break();
                    var usbWin = new UsbRequestWin();
                    usbWin.ShowPageUsbRequestNotify(usbDisk);
                    usbWin.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "USB Control");
                }
            }));
        }
        #endregion

        #region + private void Handler_FromAgentMsg_ToCloseTray()
        private void Handler_FromAgentMsg_ToCloseTray()
        {
            try
            {
                App.Current.Dispatcher.Invoke(new Action(()=> {
                    App.Current.MainWindow.Close();
                }));
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
                Debug.WriteLine(ex.Message);
#endif
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
#if DEBUG
                Debugger.Break();
                Debug.WriteLine(ex.Message);
#endif
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
#if DEBUG
                //Debugger.Break();
#endif
                // get template from http server 
                var template = new AgentHttpHelp().GetPrintTemplate_Http();

                //check FilePath(UNC) whether exist
                var templateFile = new FileInfo(template.FilePath?.Trim());
                if (!templateFile.Exists)
                {
                    throw new Exception("PrintTemplate file not exist.\r\nPath: " + template.FilePath);
                }

                // copy template file from unc to local data dir
                PrintTemplateHelp.CopyTemplateFileToLocal(templateFile);

                var json = JsonConvert.SerializeObject(new PipeMsg(PipeMsgType.AddPrintTemplate));
                _client?.PushMessage(json);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
