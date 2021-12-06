using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USBModel;

namespace USBAdminWebMVC.Controllers
{
    [Authorize]
    public class ComputerController : Controller
    {
        private readonly UsbDbHelp _usbDb;

        public ComputerController(UsbDbHelp usbDbHelp)
        {
            _usbDb = usbDbHelp;
        }

        #region ComputerIndex
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ComputerIndex(int page, int limit)
        {
            try
            {
                var (totalCount, list) = await _usbDb.Get_PerComputerList(page, limit);
                return Json(JsonResultHelp.LayuiTableData(totalCount, list));
            }
            catch (Exception ex)
            {
                return Json(JsonResultHelp.LayuiTableData(ex.Message));
            }
        }
        #endregion

        #region UsbHistory
        public async Task<IActionResult> UsbHistory(int comId)
        {
            try
            {
                var query = await _usbDb.Get_PerComputerById(comId);
                return View(query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IActionResult> UsbHistoryList(string comIdentity, int page, int limit)
        {
            try
            {
                (int totalCount, List<Tbl_PerUsbHistory> list) = await _usbDb.Get_UsbHistoryListByComputerIdentity(comIdentity, page, limit);

                return Json(JsonResultHelp.LayuiTableData(totalCount, list));
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
