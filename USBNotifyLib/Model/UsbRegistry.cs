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
            set => SetUsbRegistryKey(nameof(UsbFilterEnabled),value,RegistryValueKind.Binary);
        }

        public static string UsbFilterDbUrl
        {
            get => ReadUsbRegistryKey(nameof(UsbFilterDbUrl));
            set => SetUsbRegistryKey(nameof(UsbFilterDbUrl), value, RegistryValueKind.String);
        }

        public static string AgentDataUrl
        {
            get => ReadUsbRegistryKey(nameof(AgentDataUrl));
            set=> SetUsbRegistryKey(nameof(AgentDataUrl), value, RegistryValueKind.String);
        }

        public static int AgentTimerMinute 
        { 
            get=> Convert.ToInt32(ReadUsbRegistryKey(nameof(AgentTimerMinute)));
            set=> SetUsbRegistryKey(nameof(AgentTimerMinute), value, RegistryValueKind.DWord);
        }

        public static string VersionGuid
        {
            get => ReadUsbRegistryKey(nameof(VersionGuid));
            set => SetUsbRegistryKey(nameof(VersionGuid), value, RegistryValueKind.String);
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
