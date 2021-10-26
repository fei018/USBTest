using MadWizard.WinUSBNet;
using NativeUsbLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using USBNetLib.Win32API;

namespace USBNetLib
{
    internal partial class NotifyService
    {
        /// <summary>
        /// 保存 Task cancel token
        /// </summary>
        private CancellationTokenSource _tokenSource => new CancellationTokenSource();

        /// <summary>
        /// 保存所有 task, 可以控制取消任務
        /// </summary>
        private ConcurrentBag<Task> _notifier_Tasks => new ConcurrentBag<Task>();


        public NotifyService()
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
        #region Disk start close
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
        #endregion

        #region Notifier_Disk_Arrival
        /// <summary>
        /// Disk USB Arrival Event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Notifier_Disk_Arrival(object sender, USBEvent e)
        {
            _notifier_Tasks.Add(Task.Run(() =>
            {
                new NotifyDiskHelp().DiskHandler(e.DevicePath, out NotifyUSB usb);

            }, _tokenSource.Token));
        }
        #endregion
        #endregion
    }
}
