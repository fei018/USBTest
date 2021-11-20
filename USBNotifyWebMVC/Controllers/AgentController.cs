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

        public async Task<IActionResult> AgentData(string computerIdentity)
        {
            try
            {
                var query = await _usbDb.GetUsbAgentData(computerIdentity);
                string json = JsonConvert.SerializeObject(query);
                return Json(json);
            }
            catch (Exception)
            {
                return NoContent();
            }
        }

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
                var post = body.ReadToEndAsync().Result;

                var info = JsonHttpConvert.Deserialize_PostUserUsbHistory(post);

                await _usbDb.Update_PostUsbHistory(info);

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
