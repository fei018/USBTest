using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBCommon
{
    public interface IAgentInfoHttp
    {
        int UpdateAgentDataTimer { get; set; }

        bool UsbFilterEnabled { get; set; }

        string UsbFilterDb { get; set; }

        string ComputerIdentity { get; set; }
    }
}
