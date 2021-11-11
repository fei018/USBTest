using System;
using System.Collections.Generic;
using System.Text;

namespace USBModel
{
    public class UsbHistoryDetail
    {
        public UserUsb Usb { get; set; }

        public UserComputer Computer { get; set; }

        public UsbHistory History { get; set; }
    }
}
