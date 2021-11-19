using System;
using System.Collections.Generic;
using System.Text;
using USBCommon;

namespace USBModel
{
    public class UserAgentInfo : UserUsbFilterState, IAgentInfoHttp
    {
        public string UsbFilterDb { get; set; }
        public int UpdateAgentDataTimer { get; set; }
    }
}
