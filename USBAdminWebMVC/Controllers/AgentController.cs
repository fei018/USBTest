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
using System.Diagnostics;

namespace USBAdminWebMVC.Controllers
{
    [AgentAuthenticateFilter]
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
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }
        #endregion

        #region AgentSetting()
        public async Task<IActionResult> AgentSetting(string computerIdentity)
        {
            try
            {
                IAgentSetting query = await _usbDb.Get_AgentSetting(computerIdentity);
                string json = JsonConvert.SerializeObject(query);
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
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
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
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
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }
        #endregion

        #region RegisterUsb
        [HttpPost]      
        public async Task<IActionResult> RegisterUsb()
        {
            try
            {
                //Debugger.Break();

                StreamReader body = new StreamReader(_httpContext.Request.Body, Encoding.UTF8);
                var post = await body.ReadToEndAsync();

                PostRegisterUsb info = JsonHttpConvert.Deserialize_PostRegisterUsb(post);
                var usbRistered = info.UsbInfo as Tbl_UsbRegistered;

                await _usbDb.Register_Usb(usbRistered);

                return Json(new AgentHttpResponseResult());
            }
            catch (Exception ex)
            {
                return Json(new AgentHttpResponseResult(false,ex.Message));
            }
        }
        #endregion
    }
}
