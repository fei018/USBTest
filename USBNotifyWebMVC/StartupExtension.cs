using Microsoft.Extensions.DependencyInjection;
using USBModel;
using LoginUserManager;
using Microsoft.AspNetCore.Http;

namespace USBNotifyWebMVC
{
    public static class StartupExtension
    {
        public static void AddUSBDB(this IServiceCollection services, string connstring)
        {
            services.AddScoped(x => new LoginUserDb(connstring));
            services.AddScoped<LoginUserService>();

            services.AddScoped(x => new UsbDbHelp(connstring));

           // new UsbDbHelp(connstring).TryCreateDatabase();
        }
    }
}
