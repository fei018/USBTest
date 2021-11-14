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
                return Json(JsonResultHelp.Ok(query));
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
        public IActionResult History()
        {
            return View();
        }

        public async Task<IActionResult> HistoryIndex(int page, int limit)
        {
            try
            {
                var (totalCount, list) = await _usbDb.GetUsbHistoryDetailList(page,limit);
                return Json(JsonResultHelp.LayuiTableData(totalCount,list));
            }
            catch (Exception ex)
            {
                return Json(JsonResultHelp.LayuiTableData(ex.Message));
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
                return Ok();
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
            catch (Exception)
            {
                return Ok();
            }
        }
        #endregion

        
    }
}
