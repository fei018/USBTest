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
        private readonly USBAdminDatabaseHelp _usbDb;
        private readonly HttpContext _httpContext;
        private readonly EmailHelp _email;

        public AgentController(IHttpContextAccessor httpContextAccessor, USBAdminDatabaseHelp usbDb, EmailHelp emailHelp)
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
                var query = await _usbDb.UsbWhitelist_Get();

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
                await _usbDb.PerComputer_InsertOrUpdate(com);

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

        #region PostUsbRequest()
        [HttpPost]      
        public async Task<IActionResult> PostUsbRequest()
        {
            try
            {
                //Debugger.Break();

                StreamReader body = new StreamReader(_httpContext.Request.Body, Encoding.UTF8);
                var post = await body.ReadToEndAsync();

                Tbl_UsbRequest usbRequest = JsonHttpConvert.Deserialize_UsbRequest(post);

                await _usbDb.UsbRequest_Insert(usbRequest);

                var com = await _usbDb.PerComputer_Get_ByIdentity(usbRequest.RequestComputerIdentity);

                await _email.Send_UsbRequest_Notify_Submit_ToUser(usbRequest, com);

                return Json(new AgentHttpResponseResult());
            }
            catch(EmailException)
            {
                return Json(new AgentHttpResponseResult());
            }
            catch (Exception ex)
            {
                return Json(new AgentHttpResponseResult(false,ex.Message));
            }
        }
        #endregion


        #region AgentUpdate()
        public IActionResult AgentUpdate()
        {
            try
            {
                var file = Path.Combine(Directory.GetCurrentDirectory(), "Update", "update.zip");

                return PhysicalFile(file, "application/zip");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        #endregion
    }
}
