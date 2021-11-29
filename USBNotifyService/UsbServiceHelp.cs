using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using USBNotifyService.Win32API;

namespace USBNotifyService
{
    public partial class UsbService
    {
        private bool IsRebootUsbApp = true;

        private bool IsRebootUsbDesktop = true;

        #region ServiceStart()
        private void Start_Service()
        {
            IsRebootUsbApp = true;
            StartProcess_USBNotifyFilter();

            // 判斷當前 windows session 是否 user session
            var sessionid = ProcessApiHelp.GetCurrentUserSessionID();
            if (sessionid > 0)
            {
                IsRebootUsbDesktop = true;
                StartProcess_USBNotifyDesktop();
            }           
        }
        #endregion

        #region ServiceStop()
        private void Stop_Service()
        {
            IsRebootUsbApp = false;
            CloseProcess_USBNotifyFilter();

            IsRebootUsbDesktop = false;
            CloseProcess_USBNotifyDesktop();
        }
        #endregion

        #region USBNotifyFilter Process

        private string _agentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "usbnagent.exe");

        private Process _usbNotifyAppProcess;

        private void StartProcess_USBNotifyFilter()
        {
            CloseProcess_USBNotifyFilter();
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
                    if (IsRebootUsbApp)
                    {
                        StartProcess_USBNotifyFilter();
                    }
                };

                _usbNotifyAppProcess.Start();
            }
            catch (Exception ex)
            {
            }
        }

        private void CloseProcess_USBNotifyFilter()
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
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region USBNotityDesktop Process
        private string _desktopPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "usbndesktop.exe");

        private Process _usbNotifyDesktopProcess;

        private void StartProcess_USBNotifyDesktop()
        {
            CloseProcess_USBNotifyDesktop();

            try
            {
                _usbNotifyDesktopProcess = ProcessApiHelp.CreateProcessAsUser(_desktopPath, null);
                _usbNotifyDesktopProcess.EnableRaisingEvents = true;

                // Exited Event 委託, 如果意外結束process, 可以自己啟動
                _usbNotifyDesktopProcess.Exited += (s, e) =>
                {
                    if (IsRebootUsbDesktop)
                    {
                        StartProcess_USBNotifyDesktop();
                    }
                };
            }
            catch (Exception ex)
            {
            }
        }

        private void CloseProcess_USBNotifyDesktop()
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
            catch (Exception ex)
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
                IsRebootUsbDesktop = true;
                StartProcess_USBNotifyDesktop();
            }

            // user logoff windows
            // close Agent Desktop
            if (changeDescription.Reason == SessionChangeReason.SessionLogoff)
            {
                IsRebootUsbDesktop = false;
                CloseProcess_USBNotifyDesktop();
            }

            base.OnSessionChange(changeDescription);
        }
        #endregion
    }
}
