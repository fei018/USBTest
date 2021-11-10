using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBCommon;

namespace USBNotifyLib
{
    public class UsbHistory : IUsbHistory
    {
        public string UsbIdentity { get; set; }

        public string ComputerIdentity { get; set; }

        public DateTime PluginTime { get; set; }
    }
}
