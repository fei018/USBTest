using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBNotifyLib;
using Newtonsoft.Json;
using System.IO.Pipes;
using System.Security.AccessControl;

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

                _server = new NamedPipeServer<string>(PipeName,pipeSecurity);                

                _server.ClientMessage += _server_ClientMessage;

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
        private void _server_ClientMessage(NamedPipeConnection<string, string> connection, string message)
        {
            try
            {
                // check and update agent
                if (message == "UpdateAgent")
                {
                    Task.Run(() =>
                    {
                        new AgentUpdate().Update();
                    });
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + public void PushUsbMessage(UsbDisk usb)
        public void PushUsbMessage(UsbDisk usb)
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
