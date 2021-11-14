using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USBModel;

namespace USBNotifyWebMVC.Controllers
{
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
                var (totalCount, list) = await _usbDb.GetUserComputerList(page, limit);
                return Json(JsonResultHelp.LayuiTableData(totalCount, list));
            }
            catch (Exception ex)
            {
                return Json(JsonResultHelp.LayuiTableData(ex.Message));
            }
        }
        #endregion

        #region MyRegion
        public async Task<IActionResult> UsbHistory(int comId)
        {
            try
            {
                var query = await _usbDb.GetUserComputerById(comId);
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
                (int totalCount, List<UsbHistoryDetail> list) = await _usbDb.GetUsbHistoryDetailListByComputerIdentity(comIdentity, page, limit);

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
