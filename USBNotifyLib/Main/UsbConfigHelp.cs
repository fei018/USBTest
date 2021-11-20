using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using USBCommon;
using System.Timers;
using System.Threading.Tasks;

namespace USBNotifyLib
{
    public class UsbConfigHelp
    {
        private const string _usbRegistryKey = "SOFTWARE\\Hiphing\\USBNotify";

        #region + public static void SetAgentData(UsbAgentData agentData)
        public static void SetAgentData(UsbAgentData agentData)
        {
            try
            {
                WriteFile_UsbFilterDb(agentData.UsbFilterDb);
                SetUsbRegistryKey(nameof(UsbConfig.AgentTimerMinute), agentData.AgentTimerMinute, RegistryValueKind.DWord);
                SetUsbRegistryKey(nameof(UsbConfig.UserUsbFilterEnabled), agentData.UserUsbFilterEnabled, RegistryValueKind.Binary);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public static string[] ReadFile_UsbFilterDb()
        private static readonly object _locker_UsbFilterDb = new object();
        public static string[] ReadFile_UsbFilterDb()
        {
            lock (_locker_UsbFilterDb)
            {
                try
                {
                    if (!File.Exists(UsbConfig.UsbFilterDbFile))
                    {
                        UsbHttpHelp.GetAgentData_Http();
                    }

                    if (File.Exists(UsbConfig.UsbFilterDbFile))
                    {
                        return File.ReadAllLines(UsbConfig.UsbFilterDbFile, Encoding.UTF8);
                    }
                    return null;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
        #endregion

        #region + public static void WriteFile_UsbFilterDb(string txt)
        public static void WriteFile_UsbFilterDb(string txt)
        {
            lock (_locker_UsbFilterDb)
            {
                if (File.Exists(UsbConfig.UsbFilterDbFile))
                {
                    File.WriteAllText(UsbConfig.UsbFilterDbFile, txt, Encoding.UTF8);
                }
            }
        }
        #endregion

        #region + public static void ReloadUsbRegistryConfig()
        public static void ReloadUsbRegistryConfig()
        {
            UsbConfig.UsbFilterEnabled = Convert.ToBoolean(ReadUsbRegistryKey("UsbFilterEnabled"));
            UsbConfig.AgentDataUrl = ReadUsbRegistryKey("AgentDataUrl");
            UsbConfig.AgentTimerMinute = Convert.ToInt32(ReadUsbRegistryKey("AgentTimerMinute"));
            UsbConfig.PostUserComputerUrl = ReadUsbRegistryKey("PostUserComputerUrl");
            UsbConfig.PostUserUsbHistoryUrl = ReadUsbRegistryKey("PostUserUsbHistoryUrl");
        }
        #endregion

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

        #region + public static void SetUsbRegistryKey(string name, object value, RegistryValueKind kind)
        public static void SetUsbRegistryKey(string name, object value, RegistryValueKind kind)
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
