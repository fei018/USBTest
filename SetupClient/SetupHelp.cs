using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace SetupClient
{
    public class SetupHelp
    {
        private string _setupiniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setup.ini");

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

        #region + public void InitialKey()
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion


    }
}
