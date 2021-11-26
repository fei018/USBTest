using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using USBCommon;

namespace USBNotifyLib
{
    internal class UsbRegistry
    {
        private const string _usbRegistryKey = "SOFTWARE\\Hiphing\\USBNotify";
       

        // Registry

        public static bool UsbFilterEnabled
        {
            get => Convert.ToBoolean(ReadUsbRegistryKey(nameof(UsbFilterEnabled)));
            set => SetUsbRegistryKey(nameof(UsbFilterEnabled),value,RegistryValueKind.String);
        }

        public static bool UsbHistoryEnabled
        {
            get => Convert.ToBoolean(ReadUsbRegistryKey(nameof(UsbHistoryEnabled)));
            set => SetUsbRegistryKey(nameof(UsbHistoryEnabled), value, RegistryValueKind.String);
        }

        public static string UsbFilterDbUrl
        {
            get => ReadUsbRegistryKey(nameof(UsbFilterDbUrl));
            set => SetUsbRegistryKey(nameof(UsbFilterDbUrl), value, RegistryValueKind.String);
        }

        public static string PerAgentSettingUrl
        {
            get => ReadUsbRegistryKey(nameof(PerAgentSettingUrl));
            set => SetUsbRegistryKey(nameof(PerAgentSettingUrl), value, RegistryValueKind.String);
        }

        public static string AgentSettingUrl
        {
            get => ReadUsbRegistryKey(nameof(AgentSettingUrl));
            set=> SetUsbRegistryKey(nameof(AgentSettingUrl), value, RegistryValueKind.String);
        }

        public static int AgentTimerMinute 
        { 
            get=> Convert.ToInt32(ReadUsbRegistryKey(nameof(AgentTimerMinute)));
            set=> SetUsbRegistryKey(nameof(AgentTimerMinute), value, RegistryValueKind.String);
        }

        public static string AgentVersion
        {
            get => ReadUsbRegistryKey(nameof(AgentVersion));
            set => SetUsbRegistryKey(nameof(AgentVersion), value, RegistryValueKind.String);
        }

        public static string AgentUpdateUrl
        {
            get => ReadUsbRegistryKey(nameof(AgentUpdateUrl));
            set => SetUsbRegistryKey(nameof(AgentUpdateUrl), value, RegistryValueKind.String);
        }

        public static string PostPerComputerUrl
        {
            get=> ReadUsbRegistryKey(nameof(PostPerComputerUrl));
            set=> SetUsbRegistryKey(nameof(PostPerComputerUrl), value, RegistryValueKind.String);
        }

        public static string PostPerUsbHistoryUrl
        {
            get=> ReadUsbRegistryKey(nameof(PostPerUsbHistoryUrl));
            set=> SetUsbRegistryKey(nameof(PostPerUsbHistoryUrl), value, RegistryValueKind.String);
        }


        #region + private string ReadUsbRegistryKey(string name)
        private static string ReadUsbRegistryKey(string name)
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

        #region + private static void SetUsbRegistryKey(string name, object value, RegistryValueKind kind)
        private static void SetUsbRegistryKey(string name, object value, RegistryValueKind kind)
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
