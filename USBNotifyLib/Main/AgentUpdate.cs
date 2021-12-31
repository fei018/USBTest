using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private static string _updateDir = Path.Combine(_baseDir, "update");

        private static string _updateExe = Path.Combine(_updateDir, "update.exe");

        #region + public static void Check(string getVersion)
        public static void CheckAndUpdate(string getVersion)
        {
            try
            {
                if (!UsbRegistry.AgentVersion.Equals(getVersion, StringComparison.OrdinalIgnoreCase))
                {
                    Task.Run(() =>
                    {
                        new AgentUpdate().Update();
                    });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region + public void Update()
        public void Update()
        {
            try
            {
                CleanDownloadDir();
                DownloadFile();
                if (File.Exists(_updateExe))
                {
                    Process.Start(_updateExe);
                }
                else
                {
                    throw new Exception("update.exe not exist.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + private void CleanDownloadDir()
        private void CleanDownloadDir()
        {
            if (Directory.Exists(_downloadFileDir))
            {
                Directory.Delete(_downloadFileDir, true);
            }
            Directory.CreateDirectory(_downloadFileDir);
        }
        #endregion

        #region + private void DownloadFile()
        private void DownloadFile()
        {
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

                    ZipFile.ExtractToDirectory(_updateZipFile, _updateDir);
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
