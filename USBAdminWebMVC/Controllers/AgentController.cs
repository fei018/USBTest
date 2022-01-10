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
    [AgentKeyFilter]
    public class AgentController : Controller
    {
        private readonly UsbAdminDbHelp _usbDb;
        private readonly HttpContext _httpContext;
        private readonly EmailHelp _email;

        public AgentController(IHttpContextAccessor httpContextAccessor, UsbAdminDbHelp usbDb, EmailHelp emailHelp)
        {
            _usbDb = usbDb;
            _httpContext = httpContextAccessor.HttpContext;
            _email = emailHelp;
        }

        #region UsbWhitelist()
        public async Task<IActionResult> UsbWhitelist()
        {
            try
            {
                var query = await _usbDb.Get_UsbWhitelist();

                var agent = new AgentHttpResponseResult { Succeed = true, UsbFilterData = query };

                return Json(agent);
            }
            catch (Exception ex)
            {
                return Json(new AgentHttpResponseResult(false, ex.Message));
            }
        }
        #endregion

        #region AgentSetting()
        public async Task<IActionResult> AgentSetting()
        {
            try
            {
                IAgentSetting query = await _usbDb.Get_AgentSetting();
                var agent = new AgentHttpResponseResult { Succeed = true, AgentSetting = query };

                return Json(agent);
            }
            catch (Exception ex)
            {
                return Json(new AgentHttpResponseResult(false,ex.Message));
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

                return Json(new AgentHttpResponseResult());
            }
            catch (Exception ex)
            {
                return Json(new AgentHttpResponseResult(false,ex.Message));
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

                return Json(new AgentHttpResponseResult());
            }
            catch (Exception ex)
            {
                return Json(new AgentHttpResponseResult(false, ex.Message));
            }
        }
        #endregion

        #region UsbRegRequest()
        [HttpPost]      
        public async Task<IActionResult> UsbRegRequest()
        {
            try
            {
                //Debugger.Break();

                StreamReader body = new StreamReader(_httpContext.Request.Body, Encoding.UTF8);
                var post = await body.ReadToEndAsync();

                Tbl_UsbRegRequest usbRequest = JsonHttpConvert.Deserialize_UsbRegisterRequest(post);

                await _usbDb.Insert_UsbRegRequest(usbRequest);
                await _email.SendUsbRegisterRequestNotify(usbRequest);

                return Json(new AgentHttpResponseResult());
            }
            catch (Exception ex)
            {
                return Json(new AgentHttpResponseResult(false,ex.Message));
            }
        }
        #endregion


        #region AgentUpdate()
        public Task<IActionResult> AgentUpdate()
        {
            try
            {
                var install = Path.Combine(Directory.GetCurrentDirectory(), "Update",);

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
