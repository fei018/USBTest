using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotifyUSBWebServer.Controllers
{
    public class USBController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
