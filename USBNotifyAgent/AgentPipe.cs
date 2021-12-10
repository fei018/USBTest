using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBNotifyLib;

namespace USBNotifyAgent
{
    public class AgentPipe
    {
        private const string PipeName = "USB-b50ae7e9-5f14-4874-a273-9e119e8791e1";

        private NamedPipeServer<NotifyUsb> _server;

        public void Start()
        {
            try
            {
                _server?.Stop();

                _server = new NamedPipeServer<NotifyUsb>(PipeName);
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

        public void PushUsbMessage(NotifyUsb usb)
        {
            try
            {
                if (_server == null) throw new Exception("NamedPipeServer is null.");

                if (usb != null)
                {
                    _server.PushMessage(usb);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
