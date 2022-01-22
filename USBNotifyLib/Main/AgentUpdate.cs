﻿using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.AccessControl;

namespace USBNotifyLib
{
    public class AgentUpdate
    {
        // C:\ProgramData\USBNotify
        private string _baseDir;

        private string _downloadFileDir;

        private string _updateZipFile;

        private string _updateDir;

        private string _setupExe;

        public AgentUpdate()
        {
            _baseDir = AgentRegistry.AgentDataDir;

            _downloadFileDir = Path.Combine(_baseDir, "download");

            _updateZipFile = Path.Combine(_downloadFileDir, "update.zip");

            _updateDir = Path.Combine(_baseDir, "update");

            _setupExe = Path.Combine(_updateDir, "Setup.exe");
        }

        #region + public static void CheckAndUpdate()
        public static void CheckAndUpdate()
        {
            try
            {
                if (new AgentUpdate().CheckNeedUpdate())
                {
                    new AgentUpdate().Update();
                }
            }
            catch (Exception ex)
            {
                AgentLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + public bool CheckNeedUpdate()
        public bool CheckNeedUpdate()
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
                AgentLogger.Error(ex.Message);
                throw;
            }
        }
        #endregion

        #region + public void Update()
        public void Update()
        {
            try
            {
                CleanUpdateDir();

                DownloadFile();

                if (File.Exists(_setupExe))
                {
                    Process.Start(_setupExe);
                }
                else
                {
                    throw new Exception("setup.exe not exist.");
                }
            }
            catch (Exception ex)
            {
                AgentLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + private void CleanUpdateDir()
        private void CleanUpdateDir()
        {
            try
            {

                if (Directory.Exists(_updateDir))
                {
                    Directory.Delete(_updateDir, true);
                }
                Directory.CreateDirectory(_updateDir);
                AgentManager.SetDirACL_AuthenticatedUsers_Modify(_updateDir);

                if (Directory.Exists(_downloadFileDir))
                {
                    Directory.Delete(_downloadFileDir, true);
                }
                Directory.CreateDirectory(_downloadFileDir);
                AgentManager.SetDirACL_AuthenticatedUsers_Modify(_downloadFileDir);
            }
            catch (Exception)
            {
                throw;
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

                    // unzip download file
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
