using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using USBNotifyService.Win32API;

namespace USBNotifyService
{
    public partial class UsbService
    {
        private bool IsRebootUsbAgent = true;
        private bool IsRebootUsbAgentTray = true;

        #region ServiceStart()
        private void Start_Service()
        {
            IsRebootUsbAgent = true;
            StartProcess_USBNotifyAgent();

            // 判斷當前 windows session 是否 user session
            var sessionid = ProcessApiHelp.GetCurrentUserSessionID();
            if (sessionid > 0)
            {
                IsRebootUsbAgentTray = true;
                StartProcess_USBNotifyAgentTray();
            }
        }
        #endregion

        #region ServiceStop()
        private void Stop_Service()
        {
            IsRebootUsbAgent = false;
            CloseProcess_USBNotifyAgent();

            IsRebootUsbAgentTray = false;
            CloseProcess_USBNotifyAgentTray();
        }
        #endregion

        #region USBNotifyAgent Process

        private string _agentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "usbnagent.exe");

        private Process _usbNotifyAppProcess;

        private void StartProcess_USBNotifyAgent()
        {
            CloseProcess_USBNotifyAgent();
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(_agentPath);
                _usbNotifyAppProcess = new Process
                {
                    EnableRaisingEvents = true,
                    StartInfo = startInfo
                };

                // Exited Event 委託, 如果意外結束process, 可以自己啟動
                _usbNotifyAppProcess.Exited += (s, e) =>
                {
                    if (IsRebootUsbAgent)
                    {
                        StartProcess_USBNotifyAgent();
                    }
                };

                _usbNotifyAppProcess.Start();
            }
            catch (Exception)
            {
            }
        }

        private void CloseProcess_USBNotifyAgent()
        {
            try
            {
                if (_usbNotifyAppProcess != null && !_usbNotifyAppProcess.HasExited)
                {
                    _usbNotifyAppProcess?.CloseMainWindow();

                    if (_usbNotifyAppProcess != null && !_usbNotifyAppProcess.HasExited)
                    {
                        Thread.Sleep(new TimeSpan(0, 0, 3));
                        _usbNotifyAppProcess?.Kill();
                    }

                    _usbNotifyAppProcess?.Close();
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region USBNotityAgentTray Process
        private string _desktopPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "usbntray.exe");

        private Process _usbNotifyDesktopProcess;

        private void StartProcess_USBNotifyAgentTray()
        {
            CloseProcess_USBNotifyAgentTray();

            try
            {
                _usbNotifyDesktopProcess = ProcessApiHelp.CreateProcessAsUser(_desktopPath, null);
                _usbNotifyDesktopProcess.EnableRaisingEvents = true;

                // Exited Event 委託, 如果意外結束process, 可以自己啟動
                _usbNotifyDesktopProcess.Exited += (s, e) =>
                {
                    if (IsRebootUsbAgentTray)
                    {
                        StartProcess_USBNotifyAgentTray();
                    }
                };
            }
            catch (Exception)
            {
            }
        }

        private void CloseProcess_USBNotifyAgentTray()
        {
            try
            {
                if (_usbNotifyDesktopProcess != null && !_usbNotifyDesktopProcess.HasExited)
                {
                    _usbNotifyDesktopProcess?.CloseMainWindow();

                    if (_usbNotifyDesktopProcess != null && !_usbNotifyDesktopProcess.HasExited)
                    {
                        Thread.Sleep(new TimeSpan(0, 0, 3));
                        _usbNotifyDesktopProcess?.Kill();
                    }

                    _usbNotifyDesktopProcess?.Close();
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region + protected override void OnSessionChange(SessionChangeDescription changeDescription)
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            // user logon windows
            // startup Agent Desktop
            if (changeDescription.Reason == SessionChangeReason.SessionLogon)
            {
                IsRebootUsbAgentTray = true;
                StartProcess_USBNotifyAgentTray();
            }

            // user logoff windows
            // close Agent Desktop
            if (changeDescription.Reason == SessionChangeReason.SessionLogoff)
            {
                IsRebootUsbAgentTray = false;
                CloseProcess_USBNotifyAgentTray();
            }

            base.OnSessionChange(changeDescription);
        }
        #endregion
    }
}
