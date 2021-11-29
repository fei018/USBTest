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
        static string _installDir = Path.Combine(Environment.ExpandEnvironmentVariables("%programdata%\\USBNotity\\install"));

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
        private void InitialKey()
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region setup
        public void Install()
        {
            Console.WriteLine("Start...");
            try
            {
                InitialKey();

                var zip = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"debug.zip");
                
                ZipFile.ExtractToDirectory(zip, _USBProgramDir);

                var bat = Path.Combine(_USBProgramDir, "service_install.bat");

                Process.Start("cmd.exe", bat);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Done...");
            Console.ReadLine();
        }
        #endregion
    }
}
