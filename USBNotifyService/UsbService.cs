using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using USBNotifyLib;
using USBNotifyLib.Win32API;

namespace USBNotifyService
{
    public partial class UsbService : ServiceBase
    {
        private bool IsRebootUsbApp = true;

        public UsbService()
        {
            InitializeComponent();
            CanHandleSessionChangeEvent = true;
        }

        #region OnStart(string[] args)
        protected override void OnStart(string[] args)
        {
            IsRebootUsbApp = true;
            StartProcess_USBNotifyApp();
        }
        #endregion

        #region OnStop()
        protected override void OnStop()
        {
            IsRebootUsbApp = false;
            CloseProcess_USBNotifyApp();
        }
        #endregion


        #region USBNotifyApp Process
        private Process _usbNotifyAppProcess;

        private void StartProcess_USBNotifyApp()
        {
            try
            {
                CloseProcess_USBNotifyApp();

                ProcessStartInfo startInfo = new ProcessStartInfo(UsbConfig.USBNotifyApp);
                _usbNotifyAppProcess = new Process
                {
                    EnableRaisingEvents = true,
                    StartInfo = startInfo
                };

                // Exited Event handler, 如果意外結束process, 可以自己啟動
                _usbNotifyAppProcess.Exited += (s, e) =>
                {
                    if (IsRebootUsbApp)
                    {
                        StartProcess_USBNotifyApp();
                    }
                };

                _usbNotifyAppProcess.Start();
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }

        private void CloseProcess_USBNotifyApp()
        {
            try
            {
                if (_usbNotifyAppProcess != null && !_usbNotifyAppProcess.HasExited)
                {
                    if (!_usbNotifyAppProcess.CloseMainWindow())
                    {
                        _usbNotifyAppProcess.Kill();
                    }
                    _usbNotifyAppProcess.Close();
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

        #region MyRegion
        private Process _usbNotifyDesktopProcess;

        private void StartProcess_USBNotifyDesktop()
        {
            _usbNotifyDesktopProcess = ProcessApiHelp.CreateProcessAsUser(UsbConfig.USBNotifyDesktop,null);
            _usbNotifyDesktopProcess.EnableRaisingEvents = true;

            // Exited Event handler, 如果意外結束process, 可以自己啟動
            _usbNotifyDesktopProcess.Exited += (s, e) =>
            {
                if (IsRebootUsbApp)
                {
                    StartProcess_USBNotifyDesktop();
                }
            };           
        }

        private void CloseProcess_USBNotifyDesktop()
        {
            try
            {
                if (_usbNotifyDesktopProcess != null && !_usbNotifyDesktopProcess.HasExited)
                {
                    if (!_usbNotifyDesktopProcess.CloseMainWindow())
                    {
                        _usbNotifyDesktopProcess.Kill();
                    }
                    _usbNotifyDesktopProcess.Close();
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + protected override void OnSessionChange(SessionChangeDescription changeDescription)
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            // user logon to desktop
            if (changeDescription.Reason == SessionChangeReason.SessionLogon)
            {

            }

            base.OnSessionChange(changeDescription);
        }
        #endregion
    }
}
