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
                var query = await _usbDb.Get_UsbFilterDb(computerIdentity);
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
                IAgentSetting query = await _usbDb.Get_AgentSetting();
                string json = JsonConvert.SerializeObject(query);
                return Content(json, "application/json");
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        #endregion

        #region PerAgentSetting(string computerIdentity)
        public async Task<IActionResult> PerAgentSetting(string computerIdentity)
        {
            try
            {
                var query = await _usbDb.Get_PerAgentSetting(computerIdentity);
                string json = JsonConvert.SerializeObject(query);
                return Content(json, "application/json");
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        #endregion

        #region PostPerComputer()
        [HttpPost]
        public async Task<IActionResult> PostPerComputer()
        {
            try
            {
                StreamReader body = new StreamReader(_httpContext.Request.Body, Encoding.UTF8);
                var comjosn = await body.ReadToEndAsync();

                var com = JsonHttpConvert.Deserialize_PerComputer(comjosn);
                await _usbDb.Update_PerComputer(com);

                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        #endregion

        #region PostPerUsbHistory()
        [HttpPost]
        public async Task<IActionResult> PostPerUsbHistory()
        {
            try
            {
                StreamReader body = new StreamReader(_httpContext.Request.Body, Encoding.UTF8);
                var post = await body.ReadToEndAsync();

                var info = JsonHttpConvert.Deserialize_PerUsbHistory(post);

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
