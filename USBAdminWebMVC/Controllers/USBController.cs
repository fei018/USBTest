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
        private readonly UsbAdminDbHelp _usbDb;
        private readonly HttpContext _httpContext;

        public USBController(IHttpContextAccessor httpContextAccessor, UsbAdminDbHelp usbDb)
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
                var (totalCount, list) = await _usbDb.Get_UsbRegisteredList(page, limit);
                return JsonResultHelp.LayuiTableData(totalCount, list);
            }
            catch (Exception ex)
            {
                return JsonResultHelp.Error(ex.Message);
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
                await _usbDb.Insert_UsbRegistered(usb);
                return JsonResultHelp.Ok("Register Success.");
            }
            catch (Exception ex)
            {
                return JsonResultHelp.Error(ex.Message);
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
                return JsonResultHelp.LayuiTableData(totalCount, list);
            }
            catch (Exception ex)
            {
                return JsonResultHelp.LayuiTableData(ex.Message);
            }
        }
        #endregion

        #region RequestIndex
        public IActionResult UsbRequest()
        {
            return View();
        }

        public async Task<IActionResult> UsbRequestIndex(int page, int limit)
        {
            try
            {
                var (total, list) = await _usbDb.Get_UsbRegRequestList(page, limit);
                return JsonResultHelp.LayuiTableData(total, list);
            }
            catch (Exception ex)
            {
                return JsonResultHelp.LayuiTableData(ex.Message);
            }
        }

        public async Task<IActionResult> UsbRequestReg(int id)
        {
            try
            {
                var query = await _usbDb.Get_UsbRegRequestById(id);
                return View(query);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UsbRequestToReg(int usbRegRequestId)
        {
            try
            {
                var query = await _usbDb.Get_UsbRegRequestById(usbRegRequestId);
                await _usbDb.UsbRegRequestToRegistered(query);

                ViewBag.OK = "Register Succeed: " + query.ToString();
                return View("OK");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }
        #endregion
    }
}
