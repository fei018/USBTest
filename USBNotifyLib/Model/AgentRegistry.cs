using Microsoft.Win32;
using System;

namespace USBNotifyLib
{
    public class AgentRegistry
    {
        private const string _usbRegistryKey = "SOFTWARE\\Hiphing\\USBNotify";


        // Registry

        public static string AgentKey
        {
            get => ReadAgentRegistryKey(nameof(AgentKey));
            set => SetAgentRegistryKey(nameof(AgentKey), value, RegistryValueKind.String);
        }

        public static bool UsbFilterEnabled
        {
            get => Convert.ToBoolean(ReadAgentRegistryKey(nameof(UsbFilterEnabled)));
            set => SetAgentRegistryKey(nameof(UsbFilterEnabled), value, RegistryValueKind.String);
        }

        public static bool UsbHistoryEnabled
        {
            get => Convert.ToBoolean(ReadAgentRegistryKey(nameof(UsbHistoryEnabled)));
            set => SetAgentRegistryKey(nameof(UsbHistoryEnabled), value, RegistryValueKind.String);
        }

        public static string UsbWhitelistUrl
        {
            get => ReadAgentRegistryKey(nameof(UsbWhitelistUrl));
            set => SetAgentRegistryKey(nameof(UsbWhitelistUrl), value, RegistryValueKind.String);
        }

        public static string AgentSettingUrl
        {
            get => ReadAgentRegistryKey(nameof(AgentSettingUrl));
            set => SetAgentRegistryKey(nameof(AgentSettingUrl), value, RegistryValueKind.String);
        }

        public static int AgentTimerMinute
        {
            get => Convert.ToInt32(ReadAgentRegistryKey(nameof(AgentTimerMinute)));
            set => SetAgentRegistryKey(nameof(AgentTimerMinute), value, RegistryValueKind.String);
        }

        public static string AgentVersion
        {
            get => ReadAgentRegistryKey(nameof(AgentVersion));
            set => SetAgentRegistryKey(nameof(AgentVersion), value, RegistryValueKind.String);
        }

        public static string AgentUpdateUrl
        {
            get => ReadAgentRegistryKey(nameof(AgentUpdateUrl));
            set => SetAgentRegistryKey(nameof(AgentUpdateUrl), value, RegistryValueKind.String);
        }

        public static string PostPerComputerUrl
        {
            get => ReadAgentRegistryKey(nameof(PostPerComputerUrl));
            set => SetAgentRegistryKey(nameof(PostPerComputerUrl), value, RegistryValueKind.String);
        }

        public static string PostPerUsbHistoryUrl
        {
            get => ReadAgentRegistryKey(nameof(PostPerUsbHistoryUrl));
            set => SetAgentRegistryKey(nameof(PostPerUsbHistoryUrl), value, RegistryValueKind.String);
        }

        public static string PostUsbRequestUrl
        {
            get => ReadAgentRegistryKey(nameof(PostUsbRequestUrl));
            set => SetAgentRegistryKey(nameof(PostUsbRequestUrl), value, RegistryValueKind.String);
        }


        #region + private string ReadAgentRegistryKey(string name)
        private static string ReadAgentRegistryKey(string name)
        {
            try
            {
                using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    using (var usbKey = hklm.OpenSubKey(_usbRegistryKey))
                    {
                        var value = usbKey.GetValue(name) as string;
                        return value;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + private static void SetAgentRegistryKey(string name, object value, RegistryValueKind kind)
        private static void SetAgentRegistryKey(string name, object value, RegistryValueKind kind)
        {
            try
            {
                using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    using (var usb = hklm.CreateSubKey(_usbRegistryKey))
                    {
                        usb.SetValue(name, value, kind);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
