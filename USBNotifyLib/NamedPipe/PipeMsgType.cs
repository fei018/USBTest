using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBNotifyLib
{
    //public class PipeMsgType
    //{
    //    public const string UpdateAgent = "UpdateAgent ";

    //    public const string UpdateUsbFilterData = "UpdateUsbFilterData";

    //    public const string UpdateAgentSetting = "UpdateAgentSetting";

    //    public const string CloseAgentTray = "CloseAgentTray";

    //    public const string 
    //}

    public enum PipeMsgType
    {
        Error = 1,
        Message,
        UsbDisk,
        UpdateAgent,
        UpdateUsbWhitelist,
        UpdateAgentSetting,
        CloseAgentTray
    }
}
