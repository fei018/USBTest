using System.Threading.Tasks;
using System.Timers;
using System;

namespace USBNotifyLib
{
    public class UsbTimer
    {
        public static void RunTask()
        {
            SetTimer_GetUsbFilterDb();
            //SetTimer_GetAgentSetting();
            //SetTimer_PostUserComputer();
        }

        #region + private static void SetTimer_GetUsbFilterDb()
        private static void SetTimer_GetUsbFilterDb()
        {
            var timer = new Timer();
            timer.Interval = TimeSpan.FromMinutes(UsbRegistry.AgentTimerMinute).TotalMilliseconds;
            timer.AutoReset = true;
            timer.Elapsed += (s, e) =>
            {
                Task.Run(() =>
                {
                    UsbHttpHelp.GetUsbFilterDb_Http();
                });
            };

            timer.Enabled = true;
        }
        #endregion

        #region + private static void SetTimer_GetAgentSetting()
        private static void SetTimer_GetAgentSetting()
        {
            var timer = new Timer();
            timer.Interval = TimeSpan.FromMinutes(UsbRegistry.AgentTimerMinute).TotalMilliseconds;
            timer.AutoReset = true;
            timer.Elapsed += (s, e) =>
            {
                Task.Run(() =>
                {
                    UsbHttpHelp.GetAgentSetting_Http();
                });
            };

            timer.Enabled = true;
        }
        #endregion

        #region + private static void SetTimer_PostUserComputer()
        private static void SetTimer_PostUserComputer()
        {
            var timer = new Timer();
            timer.Interval = TimeSpan.FromMinutes(UsbRegistry.AgentTimerMinute).TotalMilliseconds;
            timer.AutoReset = true;
            timer.Elapsed += (s, e) =>
            {
                Task.Run(() =>
                {
                    UsbHttpHelp.PostUserComputer_Http();
                });
            };

            timer.Enabled = true;
        }
        #endregion

    }
}
