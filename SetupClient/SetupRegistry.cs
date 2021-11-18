using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace SetupClient
{
    public class SetupRegistry
    {
        private string _setupiniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setup.ini");

        #region + private Setupini GetSetupini()
        private Setupini GetSetupini()
        {
            try
            {
                var setup = new Setupini();
                if (File.Exists(_setupiniPath))
                {
                    var ini = File.ReadAllLines(_setupiniPath);
                    if (ini.Length <= 0) throw new Exception("Setup.ini not exist.");

                    foreach (var i in ini)
                    {
                        if (i.Contains('='))
                        {
                            SetSetupini(setup, i);
                        }                       
                    }
                }
                return setup;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetSetupini(Setupini setup, string ini)
        {
            string name = ini.Split('=')[0]?.Trim();
            string value = ini.Split('=')[1]?.Trim();

            switch (name)
            {
                case nameof(setup.PostComputerInfoUrl):
                    setup.PostComputerInfoUrl = value;
                    break;

                case nameof(setup.PostComUsbHistoryInfoUrl):
                    setup.PostComUsbHistoryInfoUrl = value;
                    break;

                case nameof(setup.UpdateTimer):
                    setup.UpdateTimer = value;
                    break;

                case nameof(setup.UsbFilterDbUrl):
                    setup.UsbFilterDbUrl = value;
                    break;

                case nameof(setup.UsbNotifyAgent):
                    setup.UsbNotifyAgent = value;
                    break;

                case nameof(setup.UsbNotifyAgentDesktop):
                    setup.UsbNotifyAgentDesktop = value;
                    break;

                case nameof(setup.UsbNotifyService):
                    setup.UsbNotifyService = value;
                    break;

                case nameof(setup.UsbRegisterUrl):
                    setup.UsbRegisterUrl = value;
                    break;

                case nameof(setup.UsbUpdateAgent):
                    setup.UsbUpdateAgent = value;
                    break;

                default:
                    break;
            }
        }
        #endregion

        public void InitialKey()
        {
            try
            {
                var setup = GetSetupini();

                using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    using (var usbKey = hklm.CreateSubKey("SOFTWARE\\Hiphing\\USBNotify",true))
                    {
                        usbKey.SetValue("UsbFilterEnabled", setup.UsbFilterEnabled, RegistryValueKind.String);
                        usbKey.SetValue("UsbFilterDbUrl", setup.UsbFilterDbUrl, RegistryValueKind.String);
                        usbKey.SetValue("PostComUsbHistoryInfoUrl", setup.PostComUsbHistoryInfoUrl, RegistryValueKind.String);
                        usbKey.SetValue("PostComputerInfoUrl", setup.PostComputerInfoUrl, RegistryValueKind.String);
                        usbKey.SetValue("UpdateTimer", setup.UpdateTimer, RegistryValueKind.String);
                        usbKey.SetValue("UsbRegisterUrl", setup.UsbRegisterUrl, RegistryValueKind.String);
                    }
                }              
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
