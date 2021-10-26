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
