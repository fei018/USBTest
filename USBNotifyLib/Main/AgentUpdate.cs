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

        private static string _updateExe = Path.Combine(_updateDir, "Setup.exe");

        #region + public static void CheckAndUpdate()
        /// <summary>
        /// run on task
        /// </summary>
        public static void CheckAndUpdate()
        {
            try
            {
                if (new AgentUpdate().IsNeedToUpdate())
                {
                    Task.Run(() =>
                    {
                        new AgentUpdate().Update();
                    });
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

        #region public bool IsNeedToUpdate()
        public bool IsNeedToUpdate()
        {
            try
            {
                using (var http = AgentHttpHelp.CreateHttpClient())
                {
                    var response = http.GetAsync(AgentRegistry.AgentSettingUrl).Result;
                    response.EnsureSuccessStatusCode();

                    var json = response.Content.ReadAsStringAsync().Result;
                    var agentResult = AgentHttpHelp.DeserialAgentResult(json);

                    if (!agentResult.Succeed)
                    {
                        throw new Exception(agentResult.Msg);
                    }

                    string newVersion = agentResult.AgentSetting.AgentVersion;

                    if (AgentRegistry.AgentVersion.Equals(newVersion, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
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
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
            CleanDownloadDir();
        }
        #endregion

        #region + private void CleanDownloadDir()
        private void CleanDownloadDir()
        {
            try
            {
                if (Directory.Exists(_downloadFileDir))
                {
                    Directory.Delete(_downloadFileDir, true);
                }
                Directory.CreateDirectory(_downloadFileDir);
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region + private void DownloadFile()
        private void DownloadFile()
        {
            using (var http = AgentHttpHelp.CreateHttpClient())
            {
                var response = http.GetAsync(AgentRegistry.AgentUpdateUrl).Result;

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
