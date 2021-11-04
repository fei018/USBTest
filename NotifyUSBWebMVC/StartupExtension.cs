using Microsoft.Extensions.DependencyInjection;
using UsbModel;

namespace NotifyUSBWebMVC
{
    public static class StartupExtension
    {
        public static void AddUSBDB(this IServiceCollection services, string connstring)
        {
            services.AddScoped(x => new UsbDbHelp(connstring));
        }
    }
}
