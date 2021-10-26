using NativeUsbLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using USBNetLib.Win32API;

namespace USBNetLib
{
    internal class NotifyDiskHelp
    {
        private readonly USBBusController _UsbBus;
        private readonly PolicyTableHelp _policyTableHelp;
        private readonly PolicyRule _policyRule;

        public NotifyDiskHelp()
        {
            _UsbBus = new USBBusController();
            _policyTableHelp = new PolicyTableHelp();
            _policyRule = new PolicyRule();
        }

        #region + public bool DiskHandler(string devicePath, out NotifyDisk disk)
        public bool DiskHandler(string devicePath, out NotifyDisk disk)
        {
            disk = new NotifyDisk { Path = devicePath };
            try
            {               
                if (Use_DiskPath_Find_UsbDeviceID_SetupDi(devicePath,out NotifyUSB notifyUsb))
                {
                    if (_UsbBus.Find_VidPidSerial_In_UsbBus(ref notifyUsb))
                    {
                        if (_policyTableHelp.IsMatchPolicyTable(ref notifyUsb))
                        {
                            disk.ParentUSB = notifyUsb;
                            USBLogger.Log(disk.ToString());
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        _policyRule.NotityUSB_NotFound_VidPidSerial_In_UsbBus(notifyUsb);
                        return false;
                    }
                }
                else
                {
                    _policyRule.DiskPath_NotFound_UsbDeviceId_SetupDi(devicePath);
                    return false;
                }              
            }
            catch (Exception ex)
            {
                USBLogger.Error(ex.Message);
                return false;
            }
        }
        #endregion


        #region SetupDi
        #region + Use_DiskPath_Find_UsbDeviceID(Guid interfaceGuid, ref NotifyUSB notifyUsb)
        /// <summary>
        /// ref NotifyUSB notifyUsb 需要賦值 notifyUsb.NotifyDevicePath
        /// </summary>
        /// <param name="notifyPath"></param>
        private bool Use_DiskPath_Find_UsbDeviceID_SetupDi(string diskPath, out NotifyUSB notifyUsb)
        {
            notifyUsb = new NotifyUSB { DiskPath = diskPath };

            Guid interfaceGuid = USetupApi.GUID_DEVINTERFACE.GUID_DEVINTERFACE_DISK;

            if (string.IsNullOrEmpty(notifyUsb.DiskPath))
                throw new Exception("notifyUsb.NotifyDevicePath IsNullOrEmpty.");

            int dicfg = (int)(USetupApi.DICFG.PRESENT | USetupApi.DICFG.DEVICEINTERFACE);
            //Guid guid = USetupApi.GUID_DEVINTERFACE.GUID_DEVINTERFACE_DISK;

            var devInfoSet = USetupApi.SetupDiGetClassDevs(ref interfaceGuid, IntPtr.Zero, IntPtr.Zero, dicfg);

            try
            {
                if (devInfoSet == USetupApi.INVALID_HANDLE_VALUE)
                    throw new Exception("SetupDiGetClassDevs handl = INVALID_HANDLE_VALUE");

                bool success = true;
                int index = 0;
                while (success)
                {
                    var devInterfaceData = new USetupApi.SP_DEVICE_INTERFACE_DATA();
                    devInterfaceData.cbSize = (uint)Marshal.SizeOf(devInterfaceData);

                    success = USetupApi.SetupDiEnumDeviceInterfaces(devInfoSet, IntPtr.Zero, ref interfaceGuid, index++, ref devInterfaceData);
                    if (success)
                    {
                        var isDetail = USetupApi.SetupDiGetDeviceInterfaceDetail(devInfoSet, ref devInterfaceData, out string devPath,
                                                                                out USetupApi.SP_DEVINFO_DATA devInfoData);
                        if (isDetail)
                        {
                            // match enum path and notify path
                            if (devPath.ToLower() == notifyUsb.DiskPath.ToLower())
                            {
                                notifyUsb.DiskDeviceID = GetDiskDeviceID(devInfoData.devInst);
                                notifyUsb.DeviceID = GetUsbDeviceIDbyChildDiskInstance(devInfoData.devInst);
                                if (string.IsNullOrEmpty(notifyUsb.DeviceID))
                                {
                                    return false;
                                }
                                else
                                {
                                    return true; // 結束循環
                                }
                            }
                        }
                    }
                }
                return false;
            }
            catch (Win32Exception)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (devInfoSet != USetupApi.INVALID_HANDLE_VALUE)
                {
                    USetupApi.SetupDiDestroyDeviceInfoList(devInfoSet);
                }
            }
        }
        #endregion

        #region + GetDiskDeviceID(uint devInst)
        private string GetDiskDeviceID(uint devInst)
        {
            if (USetupApi.CM_Get_Device_ID_Size(out uint size, devInst, 0) == 0)
            {
                StringBuilder deviceID = new StringBuilder { Length = (int)size };

                if (USetupApi.CM_Get_Device_ID(devInst, deviceID, size, 0) == 0)
                {
                    return deviceID.ToString();
                }
            }
            return null;
        }
        #endregion

        #region + GetUsbDeviceIDbyChildDiskInstance(uint devInst)
        private string GetUsbDeviceIDbyChildDiskInstance(uint devInst)
        {
            if (USetupApi.CM_Get_Parent(out uint parentInst, devInst, 0) == 0)
            {
                if (USetupApi.CM_Get_Device_ID_Size(out uint size, parentInst, 0) == 0)
                {
                    StringBuilder deviceID = new StringBuilder { Length = (int)size };

                    if (USetupApi.CM_Get_Device_ID(parentInst, deviceID, size, 0) == 0)
                    {
                        var regex = RegexDeviceIDPrefix(deviceID.ToString());

                        if (!string.IsNullOrEmpty(regex))
                        {
                            return regex;
                        }

                        return GetUsbDeviceIDbyChildDiskInstance(parentInst);
                    }
                }
            }

            return null;
        }

        private string RegexDeviceIDPrefix(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            var match = Regex.Match(path, "^USB\\\\VID_([0-9A-F]{4})&PID_([0-9A-F]{4})", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return path.Trim();
            }

            // 匹配路徑高於 USB 返回
            if (Regex.Match(path, "^PCI", RegexOptions.IgnoreCase).Success)
            {
                return null;
            }
            if (Regex.Match(path, "^ACPI", RegexOptions.IgnoreCase).Success)
            {
                return null;
            }
            if (Regex.Match(path, "^ROOT", RegexOptions.IgnoreCase).Success)
            {
                return null;
            }

            return null;
        }
        #endregion
        #endregion
    }
}
