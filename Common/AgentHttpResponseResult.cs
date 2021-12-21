using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBCommon
{
    public class AgentHttpResponseResult
    {
        public bool Succeed { get; set; }

        public string Msg { get; set; }

        public AgentHttpResponseResult(bool succeed=true, string msg = null)
        {
            Succeed = succeed;
            Msg = msg;
        }
    }
}
