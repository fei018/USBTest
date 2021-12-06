using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace USBAdminWebMVC.Controllers
{
    public class JsonResultHelp
    {
        public static object Ok(string msg = null)
        {
            return new { code = 200, msg = msg };
        }

        public static object Ok(object data, string msg = null)
        {
            return new { code = 200, data = data, msg = msg };
        }

        public static object Error(string msg)
        {
            return new { code = 400, msg = msg };
        }


        public static object LayuiTableData(int totalCount, object data)
        {
            return new { code = 0, msg = "", count = totalCount, data = data };
        }

        public static object LayuiTableData(string error)
        {
            return new { code = 400, msg = error };
        }
    }
}
