using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBNetLib
{
    internal class NotifyDisk
    {
        public string Path { get; set; }

        public NotifyUSB ParentUSB { get; set; }

        public string DeviceID
        {
            get
            {
                return ParentUSB?.NotifyDeviceID;
            }
        }
    }
}
