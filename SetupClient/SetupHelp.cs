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

        static string _USBProgramDir = Path.Combine(Environment.ExpandEnvironmentVariables("%programfiles%\\USBNotify"));

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

        #region setup
        public void Install()
        {          
            try
            {
                InitialKey();

                var zip = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"debug.zip");

                if (Directory.Exists(_USBProgramDir))
                {
                    Directory.Delete(_USBProgramDir, true);
                }

                ZipFile.ExtractToDirectory(zip, _USBProgramDir);

                var bat = WriteBatchFile();
                Console.WriteLine(bat);
                Process.Start("cmd.exe", "/c call " + "\"" + bat + "\"");
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
            sb.AppendLine($"\"C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\InstallUtil.exe\" \"{_USBProgramDir}\\usbnservice.exe\"");
            sb.AppendLine("net start usbnsrv");
            sb.AppendLine("popd");
            var InstallPath = Path.Combine(_USBProgramDir, "Service_Install.bat");
            File.WriteAllText(InstallPath, sb.ToString(), new UTF8Encoding(false));

            // service_uninstall.bat
            sb.Clear();
            sb.AppendLine("net stop usbnsrv");
            sb.AppendLine($"\"C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\InstallUtil.exe\" /u \"{_USBProgramDir}\\usbnservice.exe\"");
            sb.AppendLine("popd");
            var path = Path.Combine(_USBProgramDir, "Service_Uninstall.bat");
            File.WriteAllText(path, sb.ToString(), new UTF8Encoding(false));

            return InstallPath;
        }
        #endregion
    }
}
