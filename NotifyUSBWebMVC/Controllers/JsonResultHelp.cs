using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace USBNotifyWebMVC.Controllers
{
    public class JsonResultHelp
    {
        public static object Ok(string msg=null)
        {
            return new {code=200, msg = msg };
        }

        public static object Error(string msg)
        {
            return new {code=400, msg = msg };
        }

        public static object LayuiTableData(object data)
        {
            return new { code = 200, data = data };
        }
    }
}
