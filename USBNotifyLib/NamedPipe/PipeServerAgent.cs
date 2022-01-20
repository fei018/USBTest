using NamedPipeWrapper;
using Newtonsoft.Json;
using System;
using System.IO.Pipes;
using System.Security.AccessControl;

namespace USBNotifyLib
{
    public class PipeServerAgent
    {
        private static string PipeName = AgentRegistry.AgentHttpKey;

        private NamedPipeServer<string> _server;

        /// <summary>
        /// To Close Agent app event
        /// </summary>
        public event EventHandler CloseAgentFormEvent;

        #region + public void Start()
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

                PipeSecurity pipeSecurity = new PipeSecurity();

                pipeSecurity.AddAccessRule(new PipeAccessRule("CREATOR OWNER", PipeAccessRights.FullControl, AccessControlType.Allow));
                pipeSecurity.AddAccessRule(new PipeAccessRule("SYSTEM", PipeAccessRights.FullControl, AccessControlType.Allow));

                // Allow Everyone read and write access to the pipe.
                pipeSecurity.AddAccessRule(
                            new PipeAccessRule(
                            "Authenticated Users",
                            PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance,
                            AccessControlType.Allow));

                _server = new NamedPipeServer<string>(PipeName, pipeSecurity);

                _server.ClientMessage += ReceiveMsg_FromClientPipe;

                _server.Error += pipeConnection_Error;

                _server.Start();
            }
            catch (Exception)
            {
            }
        }

        private void pipeConnection_Error(Exception exception)
        {
            AgentLogger.Error("AgentPipe Error: " + exception.Message);
        }
        #endregion

        #region + public void Stop()
        public void Stop()
        {
            try
            {
                if (_server != null)
                {
                    _server.Error -= pipeConnection_Error;
                    _server.ClientMessage -= ReceiveMsg_FromClientPipe;
                    _server.Stop();
                    _server = null;
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion


        // Receive message from client

        #region + private void ReceiveMsg_FromClientPipe(NamedPipeConnection<string, string> connection, string message)
        private void ReceiveMsg_FromClientPipe(NamedPipeConnection<string, string> connection, string message)
        {
            try
            {
                var pipeMsg = JsonConvert.DeserializeObject<PipeMsg>(message);

                if (pipeMsg == null)
                {
                    throw new Exception("AgentPipe : PipeMsg is Null");
                }

                switch (pipeMsg.PipeMsgType)
                {
                    // check and update agent
                    case PipeMsgType.UpdateAgent:
                        Handler_UpdateAgent();
                        break;

                    // Update Setting and USB Whitelist
                    case PipeMsgType.UpdateSetting:
                        Handler_UpdateSetting();
                        break;

                    // To Close Agent
                    case PipeMsgType.CloseAgent:
                        Handler_CloseAgent();
                        break;

                    // To Close Tray
                    case PipeMsgType.CloseTray:
                        Handler_CloseTray();
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                AgentLogger.Error(ex.Message);
            }           
        }
        #endregion

        // Receive Message  handler

        #region + private void Handler_UpdateAgent()
        private void Handler_UpdateAgent()
        {
            try
            {
                if (new AgentUpdate().CheckNeedUpdate())
                {
                    new AgentUpdate().Update();
                    PushMsg_ToTray_Message("Download Agent done, wait for installation...");
                }
                else
                {
                    PushMsg_ToTray_Error("Agent is newest version.");
                }
            }
            catch (Exception ex)
            {
                AgentLogger.Error(ex.Message);
                PushMsg_ToTray_Error(ex.Message);
            }
        }
        #endregion

        #region + private void Handler_UpdateSetting()
        private void Handler_UpdateSetting()
        {
            try
            {
                new AgentHttpHelp().GetAgentSetting_Http();
                AgentTimer.ReloadTask();

                new AgentHttpHelp().GetUsbWhitelist_Http();
                new UsbFilter().Filter_Scan_All_USB_Disk();

                PushMsg_ToTray_Message("Update Setting done.");
            }
            catch (Exception ex)
            {
                AgentLogger.Error(ex.Message);
                PushMsg_ToTray_Error(ex.Message);
            }
        }
        #endregion

        #region + private void Handler_CloseAgent()
        private void Handler_CloseAgent()
        {
            try
            {
                CloseAgentFormEvent?.Invoke(this, null);
            }
            catch (Exception ex)
            {
                AgentLogger.Error("AgentPipe.Handler_CloseAgent(): " + ex.Message);
            }
        }
        #endregion

        #region + private void Handler_CloseTray()
        private void Handler_CloseTray()
        {
            try
            {
                PushMsg_ToTray_CloseTray();
            }
            catch (Exception)
            {
            }
        }
        #endregion

        // push message

        #region + public void PushMsg_ToTray_Message(string message)
        public void PushMsg_ToTray_Message(string message)
        {
            try
            {
                var pipe = new PipeMsg(PipeMsgType.Message, message);
                var json = JsonConvert.SerializeObject(pipe);
                _server.PushMessage(json);
            }
            catch (Exception ex)
            {
                AgentLogger.Error("PushMessageToTray : " + ex.Message);
            }
        }
        #endregion

        #region + public void PushMsg_ToTray_UsbDiskNotInWhitelist(UsbDisk usb)
        public void PushMsg_ToTray_UsbDiskNotInWhitelist(UsbDisk usb)
        {
            try
            {
                if (_server == null) throw new Exception("NamedPipeServer is null.");

                if (usb != null)
                {
                    var pipeMsg = new PipeMsg(usb);
                    var msgJson = JsonConvert.SerializeObject(pipeMsg);
                    _server.PushMessage(msgJson);
                }
            }
            catch (Exception ex)
            {
                AgentLogger.Error("PushMsg_ToTray_UsbDiskNotInWhitelist(UsbDisk usb) : " + ex.Message);
            }
        }
        #endregion

        #region + public void PushMsg_ToTray_Error(string message)
        public void PushMsg_ToTray_Error(string message)
        {
            try
            {
                var pipe = new PipeMsg(PipeMsgType.Error, message);
                var json = JsonConvert.SerializeObject(pipe);
                _server.PushMessage(json);
            }
            catch (Exception ex)
            {
                AgentLogger.Error("PushErrorToTray : " + ex.Message);
            }
        }
        #endregion

        #region + public void PushMsg_ToTray_CloseTray()
        public void PushMsg_ToTray_CloseTray()
        {
            try
            {
                var pipe = new PipeMsg(PipeMsgType.CloseTray);
                var json = JsonConvert.SerializeObject(pipe);
                _server.PushMessage(json);
            }
            catch (Exception ex)
            {
                AgentLogger.Error("PushMessageToCloseTray : " + ex.Message);
            }
        }
        #endregion

    }
}
