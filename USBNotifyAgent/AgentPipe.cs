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

        #region 
        public void Start()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(PipeName))
                {
                    UsbLogger.Error("PipeName is empty");
                    return;
                }

                _server?.Stop();

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

                _server.ClientMessage += _server_FromClientMessage;

                _server.Start();
            }
            catch (Exception)
            {
            }
        }

        public void Stop()
        {
            try
            {
                _server?.Stop();
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region + private void _server_ClientMessage(NamedPipeConnection<string, string> connection, string message)
        private void _server_FromClientMessage(NamedPipeConnection<string, string> connection, string message)
        {
            try
            {
                // check and update agent
                if (message == PipeMsgType.UpdateAgent)
                {
                    new AgentUpdate().Update();
                }

                // update usb filter data
                if (message == PipeMsgType.UpdateUsbFilterData)
                {
                    new AgentHttpHelp().GetUsbFilterData_Http();
                }

                // Update Agent Setting
                if (message == PipeMsgType.UpdateAgentSetting)
                {
                    new AgentHttpHelp().GetAgentSetting_Http();
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + public void PushUsbMessageToTray(UsbDisk usb)
        public void PushUsbMessageToTray(UsbDisk usb)
        {
            try
            {
                if (_server == null) throw new Exception("NamedPipeServer is null.");

                if (usb != null)
                {
                    var usbJson = JsonConvert.SerializeObject(usb);
                    _server.PushMessage(usbJson);
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

    }
}
