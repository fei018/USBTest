using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace USBAdminWebMVC
{
    public class USBAdminHelp
    {
        public static string WebHttpUrlPrefix { get; set; }

        public static string InitMenuName { get; set; }

        #region GetInitMenuJson()
        public static string GetInitMenuJson()
        {
            var manifestEmbeddedProvider = new ManifestEmbeddedFileProvider(typeof(Program).Assembly);

            var menuPath = manifestEmbeddedProvider.Assembly.GetManifestResourceNames()
                                            .Where(j => j.Contains(InitMenuName, StringComparison.OrdinalIgnoreCase))
                                            .First();

            using var menuStream = manifestEmbeddedProvider.Assembly.GetManifestResourceStream(menuPath);
            using StreamReader reader = new StreamReader(menuStream, Encoding.UTF8);
            var menuJson = reader.ReadToEnd();
            return menuJson;
        }
        #endregion

    }
}
