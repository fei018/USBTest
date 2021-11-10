using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using USBModel;

namespace USBNotifyWebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly UsbDbHelp _usbDb;

        public HomeController(UsbDbHelp usbDb)
        {
            _usbDb = usbDb;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Welcome()
        {
            try
            {
                var query = await _usbDb.GetRegisteredUsbList();
                return View("Welcome", query.Count.ToString());
            }
            catch (Exception ex)
            {
                return View("Welcome", ex.Message);
            }
        }
    }
}
