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
                case nameof(setup.PostUserComputerUrl):
                    setup.PostUserComputerUrl = value;
                    break;

                case nameof(setup.PostUserUsbHistoryUrl):
                    setup.PostUserUsbHistoryUrl = value;
                    break;

                case nameof(setup.AgentTimerMinute):
                    setup.AgentTimerMinute = uint.Parse(value);
                    break;

                case nameof(setup.AgentDataUrl):
                    setup.AgentDataUrl = value;
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

                case nameof(setup.UsbFilterEnabled):
                    setup.UsbFilterEnabled = bool.Parse(value);
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
                    using (var usbKey = hklm.CreateSubKey("SOFTWARE\\Hiphing\\USBNotify", true))
                    {
                        usbKey.SetValue("UsbFilterEnabled", setup.UsbFilterEnabled, RegistryValueKind.Binary);
                        usbKey.SetValue("UsbFilterDbUrl", setup.AgentDataUrl, RegistryValueKind.String);
                        usbKey.SetValue("PostUserUsbHistoryUrl", setup.PostUserUsbHistoryUrl, RegistryValueKind.String);
                        usbKey.SetValue("PostUserComputerUrl", setup.PostUserComputerUrl, RegistryValueKind.String);
                        usbKey.SetValue("AgentTimerMinute", setup.AgentTimerMinute, RegistryValueKind.DWord);
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
