using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using USBNetLib.Win32API;
using NativeUsbLib;
using System.Collections.Generic;
using System.Linq;

namespace USBNetLib
{
    internal class NotifySetup
    {


        #region + Use_NotifyPath_Find_USBDeviceID(Guid interfaceGuid, ref NotifyUSB notifyUsb)
        /// <summary>
        /// ref NotifyUSB notifyUsb 需要賦值 notifyUsb.NotifyDevicePath
        /// </summary>
        /// <param name="notifyPath"></param>
        public bool Use_NotifyPath_Find_USBDeviceID(Guid interfaceGuid, ref NotifyUSB notifyUsb)
        {
            if (string.IsNullOrEmpty(notifyUsb.NotifyDevicePath))
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
                            if (devPath.ToLower() == notifyUsb.NotifyDevicePath.ToLower())
                            {
                                notifyUsb.NotifyDeviceID = GetNotifyDeviceID(devInfoData.devInst);
                                notifyUsb.DeviceID = GetUSBDeviceIDbyNotifyDevice(devInfoData.devInst);

                                return true; // 結束循環
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

        #region + GetNotifyDeviceID(uint devInst)
        private string GetNotifyDeviceID(uint devInst)
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

        #region + GetUSBDeviceIDbyNotifyDevice(uint devInst)
        private string GetUSBDeviceIDbyNotifyDevice(uint devInst)
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

                        return GetUSBDeviceIDbyNotifyDevice(parentInst);
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

        #region GetNotifyUSBDevice remark Vanara.PInvoke
        //private string GetParentDeviceID(uint devInst)
        //{
        //    if (CfgMgr32.CM_Get_Parent(out uint parentInst, devInst, 0) == CfgMgr32.CONFIGRET.CR_SUCCESS)
        //    {
        //        if (CfgMgr32.CM_Get_Device_ID_Size(out uint size, parentInst, 0) == CfgMgr32.CONFIGRET.CR_SUCCESS)
        //        {
        //            StringBuilder deviceID = new StringBuilder { Length = (int)size };

        //            if (CfgMgr32.CM_Get_Device_ID(parentInst, deviceID, size, 0) == CfgMgr32.CONFIGRET.CR_SUCCESS)
        //            {
        //                var regex = RegexPath(deviceID.ToString());

        //                if (!string.IsNullOrEmpty(regex))
        //                {
        //                    return regex;
        //                }

        //                return GetParentDeviceID(parentInst);
        //            }
        //        }
        //    }

        //    return null;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public NotifyUSBDevice GetNotifyUSBDeviceByPath(string notifyDevicePath)
        //{
        //    Guid guid = SetupAPI.GUID_DEVINTERFACE_DISK;
        //    using (var devInfoSet = SetupAPI.SetupDiGetClassDevs(guid, null, IntPtr.Zero, SetupAPI.DIGCF.DIGCF_PRESENT | SetupAPI.DIGCF.DIGCF_DEVICEINTERFACE))
        //    {
        //        if (devInfoSet.IsInvalid) return null;

        //        bool success = true;
        //        uint index = 0;
        //        while (success)
        //        {
        //            var devInterfaceData = new SetupAPI.SP_DEVICE_INTERFACE_DATA();
        //            devInterfaceData.cbSize = (uint)Marshal.SizeOf(devInterfaceData);

        //            success = SetupAPI.SetupDiEnumDeviceInterfaces(devInfoSet, IntPtr.Zero, guid, index++, ref devInterfaceData);

        //            if (success)
        //            {
        //                var getDetail = SetupAPI.SetupDiGetDeviceInterfaceDetail(devInfoSet, devInterfaceData,
        //                                                                         out string devicePath,
        //                                                                         out SetupAPI.SP_DEVINFO_DATA devInfoData);

        //                if (getDetail)
        //                {
        //                    if (devicePath?.Trim().ToLower() == notifyDevicePath?.Trim().ToLower())
        //                    {
        //                        var deviceID = GetParentDeviceID(devInfoData.DevInst);
        //                        if (!string.IsNullOrEmpty(deviceID))
        //                        {
        //                            return new NotifyUSBDevice { ParentDeviceID = deviceID, DiskPath = devicePath };
        //                        }
        //                    }

        //                }
        //            }
        //        }
        //    }

        //    return null;
        //}

        //public List<NotifyUSBDevice> GetNotifyUSBDeviceList()
        //{
        //    var notifyUSBList = new List<NotifyUSBDevice>();

        //    Guid guid = SetupAPI.GUID_DEVINTERFACE_DISK;
        //    using (var devInfoSet = SetupAPI.SetupDiGetClassDevs(guid, null, IntPtr.Zero, SetupAPI.DIGCF.DIGCF_PRESENT | SetupAPI.DIGCF.DIGCF_DEVICEINTERFACE))
        //    {
        //        if (devInfoSet.IsInvalid) return null;

        //        bool success = true;
        //        uint index = 0;
        //        while (success)
        //        {
        //            var devInterfaceData = new SetupAPI.SP_DEVICE_INTERFACE_DATA();
        //            devInterfaceData.cbSize = (uint)Marshal.SizeOf(devInterfaceData);

        //            success = SetupAPI.SetupDiEnumDeviceInterfaces(devInfoSet, IntPtr.Zero, guid, index++, ref devInterfaceData);

        //            if (success)
        //            {
        //                var getDetail = SetupAPI.SetupDiGetDeviceInterfaceDetail(devInfoSet, devInterfaceData,
        //                                                                         out string devicePath,
        //                                                                         out SetupAPI.SP_DEVINFO_DATA devInfoData);

        //                if (getDetail)
        //                {
        //                    var deviceID = GetParentDeviceID(devInfoData.DevInst);
        //                    if (!string.IsNullOrEmpty(deviceID))
        //                    {
        //                        notifyUSBList.Add(new NotifyUSBDevice { ParentDeviceID = deviceID, DiskPath = devicePath });
        //                    }

        //                }
        //            }
        //        }
        //    }

        //    return notifyUSBList;
        //}
        #endregion
    }
}
