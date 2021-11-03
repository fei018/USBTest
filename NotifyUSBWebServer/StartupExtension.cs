using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USBDB;

namespace NotifyUSBWebServer
{
    public static class StartupExtension
    {
        public static void AddUSBDB(this IServiceCollection services ,string connstring)
        {
            services.AddScoped(x => new USBDbHelp(connstring));
        }
    }
}
