using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBNotifyLib;
using Newtonsoft.Json;

namespace USBNotifyAgent
{
    public class AgentPipe
    {
        private static string PipeName = UsbRegistry.AgentKey;

        private NamedPipeServer<string> _server;

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

                _server = new NamedPipeServer<string>(PipeName);
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
            catch (Exception)
            {

                throw;
            }
        }
    }
}
