using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace USBNotifyWebMVC.Controllers
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


        /// <summary>
        /// Success: code=0, Fail: code=400
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <param name="totalCount"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object LayuiTableData(int code, string msg, int totalCount, object data)
        {
            return new { code = code, msg = msg, count = totalCount, data = data };
        }
    }
}
