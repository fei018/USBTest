using MadWizard.WinUSBNet;
using NativeUsbLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using USBNetLib.Win32API;

namespace USBNetLib
{
    internal partial class NotifyHelper
    {

        private CancellationTokenSource _tokenSource => new CancellationTokenSource();

        private ConcurrentBag<Task> _notifier_Tasks => new ConcurrentBag<Task>();

        public NotifyHelper()
        {

        }

        #region Tasks
        public void Start_Notifier()
        {
            try
            {
                Start_Notifier_Disk();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Close_Notifier()
        {
            try
            {
                Close_Notifier_Disk();

                _tokenSource?.Cancel();

                //wait 10s
                Task.WhenAll(_notifier_Tasks.ToArray()).Wait(10000);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Disk Notify Task
        /// <summary>
        /// Disk Notifier
        /// </summary>
        private USBNotifier _notifier_Disk;

        /// <summary>
        /// Start Disk USB notifier
        /// </summary>
        private void Start_Notifier_Disk()
        {
            _notifier_Tasks.Add(Task.Run(() =>
            {
                Form diskForm = new Form();
                _notifier_Disk = new USBNotifier(diskForm.Handle, USetupApi.GUID_DEVINTERFACE.GUID_DEVINTERFACE_DISK);
                _notifier_Disk.Arrival += Notifier_Disk_Arrival;
                Application.Run(diskForm);

            }, _tokenSource.Token));
        }

        /// <summary>
        /// Close Disk USB notifier
        /// </summary>
        private void Close_Notifier_Disk()
        {
            if (_notifier_Disk != null)
            {
                _notifier_Disk.Arrival -= Notifier_Disk_Arrival;
                _notifier_Disk.Dispose();
            }
        }

        /// <summary>
        /// Disk USB Arrival Event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Notifier_Disk_Arrival(object sender, USBEvent e)
        {
            _notifier_Tasks.Add(Task.Run(() =>
            {
                if(IsNotifyUSBFindInPolicyTable(USetupApi.GUID_DEVINTERFACE.GUID_DEVINTERFACE_DISK, e.DevicePath, out NotifyUSB notifyUSB))
                {
                    Console.WriteLine(notifyUSB.Vid_Hex);
                    Console.WriteLine(notifyUSB.Pid_Hex);
                    Console.WriteLine(notifyUSB.SerialNumber);
                }

            }, _tokenSource.Token));
        }
        #endregion

        #region Get All USB Devices from USB Bus + ScanUsbBus(out List<Device> currentUsbList, out UsbBus bus)
        /// <summary>
        /// Get All USB Devices from USB Bus
        /// </summary>
        /// <returns></returns>
        private bool ScanUsbBus(out List<Device> currentUsbList, out UsbBus bus)
        {
            bus = new UsbBus();

            currentUsbList = new List<Device>();

            foreach (UsbController controller in bus.Controller)
            {
                if (controller != null)
                {
                    foreach (UsbHub hub in controller.Hubs)
                    {
                        if (hub != null)
                        {
                            RecursionUsb(hub.ChildDevices, ref currentUsbList);
                        }
                    }
                }

            }

            if (currentUsbList != null && currentUsbList.Count > 0)
            {
                return true;
            }
            else
            {
                DisposeUSB(ref currentUsbList, ref bus);
                return false;
            }
        }

        /// <summary>
        /// 遞歸獲取所有 usb device
        /// </summary>
        /// <param name="childDevices"></param>
        /// <param name="allDeviceList"></param>
        private void RecursionUsb(IReadOnlyCollection<Device> childDevices, ref List<Device> deviceList)
        {
            foreach (var d in childDevices)
            {
                if (d != null)
                {
                    if (!d.IsHub && !string.IsNullOrEmpty(d.DevicePath))
                    {
                        deviceList.Add(d);
                    }

                    if (d.ChildDevices != null && d.ChildDevices.Any())
                    {
                        RecursionUsb(childDevices, ref deviceList);
                    }
                }
            }
        }

        /// <summary>
        /// Dispose currentDeviceList , usbBus
        /// </summary>
        private void DisposeUSB(ref List<Device> currentUsbList, ref UsbBus bus)
        {
            try
            {
                currentUsbList?.ForEach(d =>
                {
                    d?.Dispose();
                });

                bus?.Dispose();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Is Find notify usb in policy table + IsFindNotifyUSBInPolicyTable(string devicePath)
        /// <summary>
        /// 判斷 user usb 是否在 policy table 里
        /// </summary>
        /// <param name="devicePath"></param>
        /// <returns></returns>
        private bool IsNotifyUSBFindInPolicyTable(Guid interfaceGuid, string devicePath, out NotifyUSB notifyUsb)
        {
            if (!ScanUsbBus(out List<Device> currentUsbList, out UsbBus bus))
            {
                throw new Exception("Cannot find any usb device in USB Controller."); // shall not happen
            }

            try
            {
                notifyUsb = GetNotifyUSBbyInterfaceGuidAndPath(interfaceGuid,devicePath);

                if (notifyUsb != null && !string.IsNullOrEmpty(notifyUsb.ParentDeviceID))
                {
                    return MatchPolicy(notifyUsb, ref currentUsbList);
                }
                else
                {
                    notifyUsb = null;
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally { DisposeUSB(ref currentUsbList, ref bus); }
        }


        private bool MatchPolicy(NotifyUSB notifyUsb, ref List<Device> currentUsbList)
        {
            foreach (Device d in currentUsbList)
            {
                if (d.InstanceId.Equals(notifyUsb.ParentDeviceID, StringComparison.OrdinalIgnoreCase))
                {
                    notifyUsb.Vid = d.DeviceDescriptor.idVendor;
                    notifyUsb.Pid = d.DeviceDescriptor.idProduct;
                    notifyUsb.SerialNumber = d.SerialNumber;
                }
            }

            if (notifyUsb.HasVidPidSerial)
            {
               bool? isFindIntable = PolicyTable.USBList?.Any(p =>
               {
                   return p.PID == notifyUsb.Pid && p.VID == notifyUsb.Vid && p.SerialNumber == notifyUsb.SerialNumber;
               });

                return isFindIntable ?? false;
            }
            else
            {
                throw new Exception("cannot find notity device parent in usb bus."); // shall not happen
            }
        }
        #endregion
    }
}
