using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace USBNotifyLib
{
    public class AgentUpdate
    {
        private static string _baseDir = Environment.ExpandEnvironmentVariables(@"%programdata%\USBNotify");

        private static string _downloadFileDir = Path.Combine(_baseDir, "download");

        private static string _updateZipFile = Path.Combine(_downloadFileDir, "update.zip");

        private static string _installDir = Path.Combine(_baseDir, "install");

        #region + public static void Check(string getVersion)
        public static void Check(string getVersion)
        {
            try
            {
                if (!UsbRegistry.Version.Equals(getVersion, StringComparison.OrdinalIgnoreCase))
                {
                    Task.Run(() =>
                    {
                        // execute agentupdate.exe
                    });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region + private void CleanUpdateDir()
        private void CleanUpdateDir()
        {
            if (Directory.Exists(_downloadFileDir))
            {
                Directory.Delete(_downloadFileDir, true);
            }
            Directory.CreateDirectory(_downloadFileDir);
        }
        #endregion

        #region + private void DeleteUpdateDir()
        private void DeleteUpdateDir()
        {
            if (Directory.Exists(_downloadFileDir))
            {
                Directory.Delete(_downloadFileDir, true);
            }
        }
        #endregion

        #region + private void DownloadFile()
        private void DownloadFile()
        {
            CleanUpdateDir();

            using (var http = new HttpClient())
            {
                http.Timeout = TimeSpan.FromMinutes(10);
                var response = http.GetAsync(UsbRegistry.AgentUpdateUrl).Result;

                if (response.IsSuccessStatusCode)
                {
                    var fbs = response.Content.ReadAsByteArrayAsync().Result;

                    using (FileStream fs = new FileStream(_updateZipFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        fs.Write(fbs, 0, fbs.Length);
                    }

                    ZipFile.ExtractToDirectory(_updateZipFile, _installDir);
                }
                else
                {
                    throw new Exception("download File fail.");
                }
            }
        }
        #endregion

    }
}
