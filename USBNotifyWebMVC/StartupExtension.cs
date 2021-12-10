using Microsoft.Extensions.DependencyInjection;
using USBModel;
using LoginUserManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.Unicode;
using Microsoft.Extensions.Configuration;

namespace USBNotifyWebMVC
{
    public static class StartupExtension
    {
        public static void AddMoreCustom(this IServiceCollection services, IConfiguration configuration)
        {
            USBAdminHelp.HttpUrlPrefix = configuration.GetSection("WebServerUrlPrefix").Value;
            USBAdminHelp.InitMenuName = configuration.GetSection("InitMenuName").Value;

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = new PathString("/account/login");
                options.LogoutPath = new PathString("/account/login");
                options.AccessDeniedPath = new PathString("/AccessDenied.html");
            });

            services.AddHttpContextAccessor();

            // http response html 拉丁中文不编码
            services.AddSingleton(System.Text.Encodings.Web.HtmlEncoder.Create(new[] { UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs }));
        }

        public static void AddUSBDB(this IServiceCollection services, string connstring)
        {
            services.AddScoped(x => new LoginUserDb(connstring));
            services.AddScoped<LoginUserService>();

            services.AddScoped(x => new UsbDbHelp(connstring));

           // new UsbDbHelp(connstring).TryCreateDatabase();
        }
    }
}
