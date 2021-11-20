using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBCommon
{
    public class UsbAgentData
    {
        /// <summary>
        /// unit: minute
        /// </summary>
        public int AgentTimerMinute { get; set; }

        public string UsbFilterDb { get; set; }

        public bool UserUsbFilterEnabled { get; set; }
    }
}
