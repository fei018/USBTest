using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBModel;

namespace USBNotifyWebMVC.Controllers
{
    public class USBController : Controller
    {
        private readonly UsbDbHelp _usbDb;
        private readonly HttpContext _httpContext;

        public USBController(IHttpContextAccessor httpContextAccessor, UsbDbHelp usbDb)
        {

            _usbDb = usbDb;
            _httpContext = httpContextAccessor.HttpContext;
        }

        #region RegisteredIndex
        public IActionResult Registered()
        {
            return View();
        }

        public async Task<IActionResult> RegisteredIndex()
        {
            try
            {
                var query = await _usbDb.GetRegisteredUsbList();
                return Json(JsonResultHelp.LayuiTableData(query));
            }
            catch (Exception ex)
            {
                return Json(JsonResultHelp.Error(ex.Message));
            }
        }
        #endregion

        #region Register
        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> RegisterUsb(UserUsb usb)
        {
            try
            {
                await _usbDb.Register_Usb(usb);
                return Json(JsonResultHelp.Ok("Register Success."));
            }
            catch (Exception ex)
            {
                return Json(JsonResultHelp.Error(ex.Message));
            }
        }
        #endregion

        #region FilterTable()
        public async Task<IActionResult> FilterTable()
        {
            var query = await _usbDb.GetUsbFilterTable();
            if (string.IsNullOrWhiteSpace(query))
            {
                return NotFound();
            }
            else
            {
                return Content(query, "text/plain", Encoding.UTF8);
            }
        }
        #endregion

        #region HistoryIndex
        public async Task<IActionResult> History()
        {
            try
            {
                var query = await _usbDb.GetUsbHistoryDetailList();
                return View(query);
            }
            catch (Exception ex)
            {
                ViewBag.Error(ex.Message);
                return View();
            }
        }

        public async Task<IActionResult> HistoryIndex()
        {
            try
            {
                var query = await _usbDb.GetUsbHistoryDetailList();
                return Json(JsonResultHelp.LayuiTableData(query));
            }
            catch (Exception ex)
            {
                return Json(JsonResultHelp.Error(ex.Message));
            }
        }
        #endregion

        #region PostComputerInfo()
        [HttpPost]
        public async Task<IActionResult> PostComputerInfo()
        {
            try
            {
                StreamReader body = new StreamReader(_httpContext.Request.Body, Encoding.UTF8);
                var comjosn = await body.ReadToEndAsync();

                var com = UsbJsonConvert.GetUserComputer(comjosn);
                await _usbDb.Update_UserComputer(com);

                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        #endregion

        #region PostComputerUsbHistoryInfo()
        [HttpPost]
        public async Task<IActionResult> PostComputerUsbHistoryInfo()
        {
            try
            {
                StreamReader body = new StreamReader(_httpContext.Request.Body, Encoding.UTF8);
                var post = body.ReadToEndAsync().Result;

                var info = UsbJsonConvert.GetPostComputerUsbHistoryInfo(post);

                await _usbDb.Update_PostComputerUsbHistory(info);

                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
        #endregion
    }
}
