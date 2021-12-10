using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using USBModel;

namespace USBAdminWebMVC.Controllers
{
    [Authorize]
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

        public async Task<IActionResult> RegisteredIndex(int page, int limit)
        {
            try
            {
                var query = await _usbDb.Get_UsbRegisteredList(page, limit);
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

        public async Task<IActionResult> RegisterUsb(Tbl_UsbRegistered usb)
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

        #region HistoryIndex
        public IActionResult History()
        {
            return View();
        }

        public async Task<IActionResult> HistoryIndex(int page, int limit)
        {
            try
            {
                var (totalCount, list) = await _usbDb.Get_UsbHistoryVMList(page, limit);
                return Json(JsonResultHelp.LayuiTableData(totalCount, list));
            }
            catch (Exception ex)
            {
                return Json(JsonResultHelp.LayuiTableData(ex.Message));
            }
        }
        #endregion       
    }
}
