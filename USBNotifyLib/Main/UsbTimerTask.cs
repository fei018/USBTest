using System.Threading.Tasks;
using System.Timers;

namespace USBNotifyLib
{
    public class UsbTimerTask
    {
        public static void Run()
        {
            SetTimer_AgentData();
            SetTimer_PostUserComputer();
        }

        #region + private static void SetTimer_AgentData()
        private static void SetTimer_AgentData()
        {
            var timer = new Timer();
            timer.Interval = UsbConfig.AgentTimerMinute;
            timer.AutoReset = false;
            timer.Elapsed += (s, e) =>
            {
                Task.Run(() =>
                {
                    UsbHttpHelp.GetAgentData_Http();
                });
            };

            timer.Enabled = true;
        }
        #endregion

        #region + private static void SetTimer_PostUserComputer()
        private static void SetTimer_PostUserComputer()
        {
            var timer = new Timer();
            timer.Interval = UsbConfig.AgentTimerMinute;
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
