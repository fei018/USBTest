using System.Threading.Tasks;
using System.Timers;

namespace USBNotifyLib
{
    public class UsbTimer
    {
        public static void RunTask()
        {
            SetTimer_GetUsbFilterDb();
            SetTimer_PostUserComputer();
        }

        #region + private static void SetTimer_GetUsbFilterDb()
        private static void SetTimer_GetUsbFilterDb()
        {
            var timer = new Timer();
            timer.Interval = UsbRegistry.AgentTimerMinute;
            timer.AutoReset = false;
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

        #region + private static void SetTimer_PostUserComputer()
        private static void SetTimer_PostUserComputer()
        {
            var timer = new Timer();
            timer.Interval = UsbRegistry.AgentTimerMinute;
            timer.AutoReset = false;
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
