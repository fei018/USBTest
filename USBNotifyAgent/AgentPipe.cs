using NamedPipeWrapper;
using Newtonsoft.Json;
using System;
using System.IO.Pipes;
using System.Security.AccessControl;
using USBNotifyLib;

namespace USBNotifyAgent
{
    public class AgentPipe
    {
        private static string PipeName = AgentRegistry.AgentKey;

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
                    UsbLogger.Error("PipeName is empty");
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
            UsbLogger.Error("AgentPipe Error: " + exception.Message);
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

                    // update usb whitelist
                    case PipeMsgType.UpdateUsbWhitelist:
                        Handler_UpdateUsbWhitelist();
                        break;

                    // Update Agent Setting
                    case PipeMsgType.UpdateAgentSetting:
                        Handler_UpdateAgentSetting();
                        break;

                    // Usb Full Scan
                    case PipeMsgType.UsbFullScan:
                        Handler_UsbFullScan();
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
                UsbLogger.Error(ex.Message);
            }           
        }
        #endregion

        // Receive Message  handler

        #region + private void Handler_UpdateUsbWhitelist()
        private void Handler_UpdateUsbWhitelist()
        {
            try
            {
                new AgentHttpHelp().GetUsbWhitelist_Http();
                PushMsg_ToTray_Message("Update USB Whitelist done.");

                new UsbFilter().Filter_Scan_All_USB_Disk();
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
                PushMsg_ToTray_Error(ex.Message);
            }
        }
        #endregion

        #region + private void Handler_UpdateAgent()
        private void Handler_UpdateAgent()
        {
            try
            {
                if (new AgentUpdate().IsNeedToUpdate())
                {
                    new AgentUpdate().Update();
                    PushMsg_ToTray_Message("Download Agent done, wait for installation.");
                }
                else
                {
                    PushMsg_ToTray_Error("Agent is newest version.");
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
                PushMsg_ToTray_Error(ex.Message);
            }
        }
        #endregion

        #region + private void Handler_UpdateAgentSetting()
        private void Handler_UpdateAgentSetting()
        {
            try
            {
                new AgentHttpHelp().GetAgentSetting_Http();
                AgentTimer.ReloadTask();
                PushMsg_ToTray_Message("Update AgentSetting done.");
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
                PushMsg_ToTray_Error(ex.Message);
            }
        }
        #endregion

        #region + private void Handler_UsbFullScan()
        private void Handler_UsbFullScan()
        {
            try
            {
                new UsbFilter().Filter_Scan_All_USB_Disk();
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
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
                UsbLogger.Error("AgentPipe.Handler_CloseAgent(): " + ex.Message);
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
                UsbLogger.Error("PushMessageToTray : " + ex.Message);
            }
        }
        #endregion

        #region + public void PushMsg_ToTray_UsbDisk(UsbDisk usb)
        public void PushMsg_ToTray_UsbDisk(UsbDisk usb)
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
                UsbLogger.Error("PushUsbDiskToTray: " + ex.Message);
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
                UsbLogger.Error("PushErrorToTray : " + ex.Message);
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
                UsbLogger.Error("PushMessageToCloseTray : " + ex.Message);
            }
        }
        #endregion

    }
}
