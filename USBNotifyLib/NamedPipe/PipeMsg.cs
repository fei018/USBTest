using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBNotifyLib
{
    public class PipeMsg
    {
        public UsbDisk UsbDisk { get; set; }

        public string Error { get; set; }

        public PipeMsg(string error=null)
        {
            Error = error;
        }

        public PipeMsg(UsbDisk usbDisk)
        {
            UsbDisk = usbDisk;
        }
    }
}
