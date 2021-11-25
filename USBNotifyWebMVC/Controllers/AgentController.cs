using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USBModel;
using USBCommon;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;

namespace USBNotifyWebMVC.Controllers
{
    public class AgentController : Controller
    {
        private readonly UsbDbHelp _usbDb;
        private readonly HttpContext _httpContext;

        public AgentController(IHttpContextAccessor httpContextAccessor, UsbDbHelp usbDb)
        {
            _usbDb = usbDb;
            _httpContext = httpContextAccessor.HttpContext;
        }

        #region UsbFilterDb(string computerIdentity)
        public async Task<IActionResult> UsbFilterDb(string computerIdentity)
        {
            try
            {
                var query = await _usbDb.GetUsbFilterDb(computerIdentity);
                string json = JsonConvert.SerializeObject(query);
                return Content(json, "application/json");
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        #endregion

        #region AgentSetting()
        public async Task<IActionResult> AgentSetting()
        {
            try
            {
                IAgentSetting query = await _usbDb.GetAgentSetting();
                string json = JsonConvert.SerializeObject(query);
                return Content(json, "application/json");
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        #endregion

        #region PostUserComputer()
        [HttpPost]
        public async Task<IActionResult> PostUserComputer()
        {
            try
            {
                StreamReader body = new StreamReader(_httpContext.Request.Body, Encoding.UTF8);
                var comjosn = await body.ReadToEndAsync();

                var com = JsonHttpConvert.Deserialize_UserComputer(comjosn);
                await _usbDb.Update_UserComputer(com);

                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        #endregion

        #region PostUserUsbHistory()
        [HttpPost]
        public async Task<IActionResult> PostUserUsbHistory()
        {
            try
            {
                StreamReader body = new StreamReader(_httpContext.Request.Body, Encoding.UTF8);
                var post = await body.ReadToEndAsync();

                var info = JsonHttpConvert.Deserialize_UserUsbHistory(post);

                await _usbDb.Insert_UsbHistory(info);

                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        #endregion
    }
}
