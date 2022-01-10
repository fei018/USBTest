using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace SetupClient
{
    public class SetupHelp
    {

        static string _setupiniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setup.ini");

        static string _InstallProgramDir = Path.Combine(Environment.ExpandEnvironmentVariables("%programfiles%\\USBNotify"));

        static string _installServiceBatch = Path.Combine(_InstallProgramDir, "Service_Install.bat");

        static string _uninstallServiceBatch = Path.Combine(_InstallProgramDir, "Service_Uninstall.bat");

        #region + private Setupini GetSetupini()
        private Dictionary<string, string> GetSetupini()
        {
            try
            {
                Dictionary<string, string> registry = new Dictionary<string, string>();

                if (File.Exists(_setupiniPath))
                {
                    var ini = File.ReadAllLines(_setupiniPath);
                    if (ini.Length <= 0) throw new Exception("Setup.ini not exist.");

                    int count = -1;

                    for (int i = 0; i < ini.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(ini[i]))
                        {
                            if (ini[i].ToLower() == "[registry]")
                            {
                                count = i;
                                continue;
                            }
                            if (count >= 0)
                            {
                                if (Regex.IsMatch(ini[i].Trim().ToLower(), "\\[[a-z]\\]"))
                                {
                                    break;
                                }

                                if (ini[i].Contains('='))
                                {
                                    registry.Add(ini[i].Split('=')[0].Trim(), ini[i].Split('=')[1].Trim());
                                }
                            }
                        }
                    }
                }
                return registry;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + private void InitialKey()
        public void InitialKey()
        {
            try
            {
                var setup = GetSetupini();

                // Registry key location: Computer\HKEY_LOCAL_MACHINE
                using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    var key = "SOFTWARE\\Hiphing\\USBNotify";

                    // delete old key
                    hklm.DeleteSubKey(key, false);

                    using (var usbKey = hklm.CreateSubKey(key, true))
                    {
                        foreach (var s in setup)
                        {
                            usbKey.SetValue(s.Key, s.Value, RegistryValueKind.String);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region unsetup

        #endregion

        #region setup
        public void Install()
        {          
            try
            {
                InitialKey();

                var updateDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"update");

                if (Directory.Exists(_InstallProgramDir))
                {
                    Directory.Delete(_InstallProgramDir, true);
                }

                var files = Directory.GetFiles(updateDir);
                foreach (var f in files)
                {
                    File.Copy(f, Path.Combine(_InstallProgramDir, Path.GetFileName(f)), true);
                }

                var installServiceBatch = WriteBatchFile();
                Console.WriteLine(installServiceBatch);
                Process.Start("cmd.exe", "/c call " + "\"" + installServiceBatch + "\"");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string WriteBatchFile()
        {
            var sb = new StringBuilder();

            // service_install.bat
            sb.AppendLine($"\"C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\InstallUtil.exe\" \"{_InstallProgramDir}\\usbnservice.exe\"");
            sb.AppendLine("net start usbnsrv");
            sb.AppendLine("popd");
            File.WriteAllText(_installServiceBatch, sb.ToString(), new UTF8Encoding(false));

            // service_uninstall.bat
            sb.Clear();
            sb.AppendLine("net stop usbnsrv");
            sb.AppendLine($"\"C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\InstallUtil.exe\" /u \"{_InstallProgramDir}\\usbnservice.exe\"");
            sb.AppendLine("popd");
            File.WriteAllText(_uninstallServiceBatch, sb.ToString(), new UTF8Encoding(false));

            return _installServiceBatch;
        }
        #endregion
    }
}
