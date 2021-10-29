using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using NotifyUSBFormApp;
using USBNetLib;

namespace NotifyUsbWinService
{
    public partial class NUWAppService : ServiceBase
    {
        public NUWAppService()
        {
            InitializeComponent();
        }

        private Process _usbFormProcess;
        private bool IsBootUsbForm = true;

        protected override void OnStart(string[] args)
        {
            IsBootUsbForm = true;
            StartUsbFormProcess();
        }

        protected override void OnStop()
        {
            IsBootUsbForm = false;
            CloseUsbFormProcess();
        }

        private void StartUsbFormProcess()
        {
            try
            {
                CloseUsbFormProcess();

                ProcessStartInfo startInfo = new ProcessStartInfo(NotifyUSBForm.NotifyUSBAppPath);
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
                USBLogger.Error(ex.Message);
            }
        }

        private void CloseUsbFormProcess()
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
                USBLogger.Error(ex.Message);
            }
        }

        /// <summary>
        /// 意外結束process, 可以自己啟動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void usbProcess_Exited(object sender, EventArgs e)
        {
            if (IsBootUsbForm)
            {
                StartUsbFormProcess();
            }

            USBLogger.Log("USBFormApp Process Exied Event.");
        }
    }
}
