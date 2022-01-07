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

                _server.ClientMessage += ReceiveMessageFromTrayPipe;

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
                    _server.ClientMessage -= ReceiveMessageFromTrayPipe;
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

        #region + private void ReceiveMessageFromTrayPipe(NamedPipeConnection<string, string> connection, string message)
        private void ReceiveMessageFromTrayPipe(NamedPipeConnection<string, string> connection, string message)
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

        // ReceiveMessageFromTrayPipe handler

        #region + private void Handler_UpdateUsbWhitelist()
        private void Handler_UpdateUsbWhitelist()
        {
            try
            {
                new AgentHttpHelp().GetUsbWhitelist_Http();
                PushMessageToTray("Update USB Whitelist done.");
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
                PushErrorToTray(ex.Message);
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
                    PushMessageToTray("Update Agent done.");
                }
                else
                {
                    PushErrorToTray("Agent is newest version.");
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
                PushErrorToTray(ex.Message);
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
                PushMessageToTray("Update AgentSetting done.");
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
                PushErrorToTray(ex.Message);
            }
        }
        #endregion

        // push message to tray pipe client

        #region + public void PushMessageToTray(string message)
        public void PushMessageToTray(string message)
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

        #region + public void PushUsbDiskToTray(UsbDisk usb)
        public void PushUsbDiskToTray(UsbDisk usb)
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

        #region + public void PushErrorToTray(string message)
        public void PushErrorToTray(string message)
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

    }
}
