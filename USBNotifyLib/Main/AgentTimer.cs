using System.Threading.Tasks;
using System.Timers;
using System;

namespace USBNotifyLib
{
    public class AgentTimer
    {
        public static void RunTask()
        {
            SetTimerTask();
        }


        #region + private static void SetTimerTask()
        private static void SetTimerTask()
        {
            try
            {
                var timerTask = new Timer();
                timerTask.Interval = TimeSpan.FromMinutes(AgentRegistry.AgentTimerMinute).TotalMilliseconds;
                timerTask.AutoReset = true;
                timerTask.Elapsed += TimerTask_Elapsed;

                timerTask.Enabled = true;                            
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }

        private static void TimerTask_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimerTask_PostPerComputerTimer();
            TimerTask_Get_AgentSetting();
            TimerTask_CheckAndUpdateAgent();
        }
        #endregion

        #region + private static void TimerTask_Get_AgentSetting()
        private static void TimerTask_Get_AgentSetting()
        {
            Task.Run(() =>
            {
                try
                {
                    new AgentHttpHelp().GetAgentSetting_Http();

                    if (AgentRegistry.UsbFilterEnabled)
                    {
                        new AgentHttpHelp().GetUsbFilterData_Http();
                    }
                }
                catch (Exception ex)
                {
                    UsbLogger.Error(ex.Message);
                }
            });
        }
        #endregion

        #region + private static void TimerTask_PostPerComputerTimer()
        private static void TimerTask_PostPerComputerTimer()
        {
            Task.Run(() =>
            {
                try
                {
                    new AgentHttpHelp().PostPerComputer_Http();
                }
                catch (Exception ex)
                {
                    UsbLogger.Error(ex.Message);
                }
            });
        }
        #endregion

        #region + private static void TimerTask_CheckAndUpdateAgent()
        private static void TimerTask_CheckAndUpdateAgent()
        {
            AgentUpdate.CheckAndUpdate();
        }
        #endregion
    }
}
