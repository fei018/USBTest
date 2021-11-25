using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using USBModel;

namespace USBNotifyWebMVC.Controllers
{
    [Authorize]
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
                var query = await _usbDb.GetRegisteredUsbCount();
                return View("Welcome", query.ToString());
            }
            catch (Exception ex)
            {
                return View("Welcome", ex.Message);
            }
        }
    }
}
