﻿using Microsoft.AspNetCore.Http;
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
            _usbDb.TryCreateDatabase();
            _httpContext = httpContextAccessor.HttpContext;
        }

        #region Index
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> IndexUsb()
        {
            try
            {
                var query = await _usbDb.GetRegisteredUsbList();
                return Json(JsonHelp.LayuiTableData(query));
            }
            catch (Exception ex)
            {
                return Json(JsonHelp.Error(ex.Message));
            }
        }
        #endregion

        #region Register
        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> RegisterUsb(UsbInfo usb)
        {
            try
            {
                await _usbDb.RegisterUsb(usb);
                return Json(JsonHelp.Ok("Register Success."));
            }
            catch (Exception ex)
            {
                return Json(JsonHelp.Error(ex.Message));
            }
        }
        #endregion

        #region GetUsbFilterTable
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

        #region PostComputerInfo
        [HttpPost]
        public async Task<IActionResult> PostComputerInfo()
        {
            StreamReader body = new StreamReader(_httpContext.Request.Body, Encoding.UTF8);
            var comjosn = await body.ReadToEndAsync();

            await _usbDb.UpdateOrInsert_ComputerInfo_by_Json(comjosn);

            return Ok();
        }
        #endregion


    }
}
