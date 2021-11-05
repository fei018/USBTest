using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBModel;

namespace NotifyUSBWebMVC.Controllers
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

        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> UpdateUsbFilterTable()
        {
            var query = await _usbDb.GetUsbFilterTable();
            if (string.IsNullOrWhiteSpace(null))
            {
                return NotFound();
            }
            else
            {
                return Content(query, "text/plain", Encoding.UTF8);
            }
        }

        public async Task<IActionResult> UpdateComputerInfo()
        {
            StreamReader body = new StreamReader(_httpContext.Request.Body, Encoding.UTF8);
            var comjosn = await body.ReadToEndAsync();

            await _usbDb.UpdateOrInsert_ComputerInfo_by_Json(comjosn);

            return Ok();
        }
    }
}
