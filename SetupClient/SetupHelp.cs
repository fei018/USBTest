using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace SetupClient
{
    public class SetupHelp
    {
        static string _InstallProgramDir = Path.Combine(Environment.ExpandEnvironmentVariables("%programfiles%\\USBNotify"));


        static string InstallUtilExe = "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\InstallUtil.exe";

        static string _serviceExe = Path.Combine(_InstallProgramDir, "usbnservice.exe");

        static string _setupDir = AppDomain.CurrentDomain.BaseDirectory;

        static string _setupiniPath = Path.Combine(_setupDir, "setup.ini");

        

        static string _installServiceBatch = Path.Combine(_InstallProgramDir, "Service_Install.bat");

        static string _uninstallServiceBatch = Path.Combine(_InstallProgramDir, "Service_Uninstall.bat");

        static string _dllDir = Path.Combine(_setupDir, "dll");

        static string _registryKey = "SOFTWARE\\Hiphing\\USBNotify";

        #region + private Setupini GetSetupini()
        private Dictionary<string, string> GetSetupini()
        {
            try
            {
#if DEBUG
                _setupiniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setupDebug.ini");
                Console.WriteLine(_setupiniPath);
#endif

                Dictionary<string, string> registry = new Dictionary<string, string>();

                var iniInfo = new FileInfo(_setupiniPath);
                if (!iniInfo.Exists)
                {
                    throw new Exception("Setup.ini not exist.");
                }

                var ini = File.ReadAllLines(_setupiniPath);
                if (ini.Length <= 0) throw new Exception("Setup.ini not exist.");

                int count = -1;

                for (int i = 0; i < ini.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(ini[i]))
                    {
                        if (ini[i].Trim().ToLower() == "[registry]")
                        {
                            count = i;
                            continue;
                        }
                        if (count >= 0)
                        {
                            if (Regex.IsMatch(ini[i].Trim().ToLower(), "\\[[a-z]{1,}\\]"))
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

                return registry;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + private void InitialRegistryKey()
        public void InitialRegistryKey()
        {
            try
            {
                var setup = GetSetupini();

                // Registry key location: Computer\HKEY_LOCAL_MACHINE
                using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    var key = _registryKey;

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

        #region + private void CreateAndCopyInstallDir()
        private void CreateAndCopyInstallDir()
        {
            if (Directory.Exists(_InstallProgramDir))
            {
                Directory.Delete(_InstallProgramDir, true);
            }

            var dir = Directory.CreateDirectory(_InstallProgramDir);
            var dirACL = dir.GetAccessControl();

            var rule = new FileSystemAccessRule("Authenticated Users",
                            FileSystemRights.ReadAndExecute,
                            InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                            PropagationFlags.None,
                            AccessControlType.Allow);
            dirACL.AddAccessRule(rule);
            dir.SetAccessControl(dirACL);

            var files = Directory.GetFiles(_dllDir);
            foreach (var f in files)
            {
                File.Copy(f, Path.Combine(_InstallProgramDir, Path.GetFileName(f)), true);
            }
        }
        #endregion

        #region + public void Install()
        public void Install()
        {
            try
            {
                UninstallService(out string error);
                Console.WriteLine(error);

                CreateAndCopyInstallDir();

                WriteBatchFile();

                InitialRegistryKey();

                InstallService(out error);
                Console.WriteLine(error);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + private bool InstallService(out string error)
        private bool InstallService(out string error)
        {
            var start = new ProcessStartInfo();
            start.FileName = "cmd.exe";
            start.UseShellExecute = false;
            start.WorkingDirectory = Environment.CurrentDirectory;
            start.CreateNoWindow = true;
            start.RedirectStandardError = true;
            start.RedirectStandardInput = true;
            start.RedirectStandardOutput = true;

            using (Process p = new Process())
            {
                p.StartInfo = start;

                var run = p.Start();

                p.StandardInput.WriteLine($"\"{InstallUtilExe}\" \"{_serviceExe}\"");
                Thread.Sleep(new TimeSpan(0, 0, 5));

                p.StandardInput.WriteLine("net start usbnservice");
                Thread.Sleep(new TimeSpan(0, 0, 2));

                p.StandardInput.WriteLine("exit");

                error = p.StandardError.ReadToEnd();

                return run;
            }

        }
        #endregion

        #region + private bool UninstallService(out string error)
        private bool UninstallService(out string error)
        {
            var serviceExist = ServiceController.GetServices().Any(s => s.ServiceName == "usbnservice");
            if (!serviceExist)
            {
                error = null;
                return true;
            }

            var start = new ProcessStartInfo();
            start.FileName = "cmd.exe";
            start.UseShellExecute = false;
            start.WorkingDirectory = Environment.CurrentDirectory;
            start.CreateNoWindow = true;
            start.RedirectStandardError = true;
            start.RedirectStandardInput = true;
            start.RedirectStandardOutput = true;

            using (Process p = new Process())
            {
                p.StartInfo = start;

                var run = p.Start();

                p.StandardInput.WriteLine("net stop usbnservice");
                Thread.Sleep(new TimeSpan(0, 0, 2));

                p.StandardInput.WriteLine($"\"{InstallUtilExe}\" /u \"{_serviceExe}\"");
                Thread.Sleep(new TimeSpan(0, 0, 5));

                p.StandardInput.WriteLine("exit");

                error = p.StandardError.ReadToEnd();

                return run;
            }

        }
        #endregion

        #region WriteBatchFile
        private string WriteBatchFile()
        {
            var sb = new StringBuilder();

            // service_install.bat
            sb.AppendLine($"\"{InstallUtilExe}\" \"{_serviceExe}\"");
            sb.AppendLine("net start usbnservice");

            File.WriteAllText(_installServiceBatch, sb.ToString(), new UTF8Encoding(false));

            // service_uninstall.bat
            sb.Clear();
            sb.AppendLine("net stop usbnservice");
            sb.AppendLine($"\"{InstallUtilExe}\" /u \"{_serviceExe}\"");
            File.WriteAllText(_uninstallServiceBatch, sb.ToString(), new UTF8Encoding(false));

            return _installServiceBatch;
        }
        #endregion
    }
}
