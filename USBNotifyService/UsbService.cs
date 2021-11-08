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

namespace USBNotifyService
{
    public partial class UsbService : ServiceBase
    {
        public UsbService()
        {
            InitializeComponent();
            CanHandleSessionChangeEvent = true;
        }

        private Process _usbFormProcess;
        private bool IsBootUsbForm = true;

        protected override void OnStart(string[] args)
        {
            IsBootUsbForm = true;
            StartUSBNotifyAppProcess();
        }

        protected override void OnStop()
        {
            IsBootUsbForm = false;
            CloseUSBNotifyAppProcess();
        }

        #region USBNotifyApp Process
        private void StartUSBNotifyAppProcess()
        {
            try
            {
                CloseUSBNotifyAppProcess();

                ProcessStartInfo startInfo = new ProcessStartInfo(UsbConfig.NUWAppPath);
                _usbFormProcess = new Process
                {
                    EnableRaisingEvents = true,
                    StartInfo = startInfo
                };

                _usbFormProcess.Exited += usbProcess_Exited;

                _usbFormProcess.Start();
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }

        private void CloseUSBNotifyAppProcess()
        {
            try
            {
                if (_usbFormProcess != null && !_usbFormProcess.HasExited)
                {
                    if (!_usbFormProcess.CloseMainWindow())
                    {
                        _usbFormProcess.Kill();
                    }
                    _usbFormProcess.Close();
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }

        /// <summary>
        /// 如果意外結束process, 可以自己啟動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void usbProcess_Exited(object sender, EventArgs e)
        {
            if (IsBootUsbForm)
            {
                StartUSBNotifyAppProcess();
            }

            UsbLogger.Log("USBFormApp Process Exited Event.");
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
