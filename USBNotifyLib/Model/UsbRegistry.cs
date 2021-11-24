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

        public static string UsbFilterDbUrl
        {
            get => ReadUsbRegistryKey(nameof(UsbFilterDbUrl));
            set => SetUsbRegistryKey(nameof(UsbFilterDbUrl), value, RegistryValueKind.String);
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

        public static string Version
        {
            get => ReadUsbRegistryKey(nameof(Version));
            set => SetUsbRegistryKey(nameof(Version), value, RegistryValueKind.String);
        }

        public static string AgentUpdateUrl
        {
            get => ReadUsbRegistryKey(nameof(AgentUpdateUrl));
            set => SetUsbRegistryKey(nameof(AgentUpdateUrl), value, RegistryValueKind.String);
        }

        public static string PostUserComputerUrl
        {
            get=> ReadUsbRegistryKey(nameof(PostUserComputerUrl));
            set=> SetUsbRegistryKey(nameof(PostUserComputerUrl), value, RegistryValueKind.String);
        }

        public static string PostUserUsbHistoryUrl
        {
            get=> ReadUsbRegistryKey(nameof(PostUserUsbHistoryUrl));
            set=> SetUsbRegistryKey(nameof(PostUserUsbHistoryUrl), value, RegistryValueKind.String);
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
