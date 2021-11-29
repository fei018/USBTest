﻿using System.Threading.Tasks;
using System.Timers;
using System;

namespace USBNotifyLib
{
    public class UsbTimer
    {
        public static void RunTask()
        {
            SetTimer_GetAgentSetting();
            SetTimer_PostUserComputer();
        }


        #region + private static void SetTimer_GetAgentSetting()
        private static void SetTimer_GetAgentSetting()
        {
            try
            {
                var timer = new Timer();
                timer.Interval = TimeSpan.FromMinutes(UsbRegistry.AgentTimerMinute).TotalMilliseconds;
                timer.AutoReset = true;
                timer.Elapsed += (s, e) =>
                {
                    new UsbHttpHelp().GetAgentSetting_Http();
                };

                timer.Enabled = true;
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + private static void SetTimer_PostUserComputer()
        private static void SetTimer_PostUserComputer()
        {
            try
            {
                var timer = new Timer();
                timer.Interval = TimeSpan.FromMinutes(UsbRegistry.AgentTimerMinute).TotalMilliseconds;
                timer.AutoReset = true;
                timer.Elapsed += (s, e) =>
                {
                    new UsbHttpHelp().PostPerComputer_Http();
                };

                timer.Enabled = true;
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

    }
}
