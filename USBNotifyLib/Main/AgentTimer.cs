using System.Threading.Tasks;
using System.Timers;
using System;

namespace USBNotifyLib
{
    public class AgentTimer
    {
        private static Timer _Timer;

        public static void ReloadTask()
        {
            ClearTimerTask();
            SetTimerTask();
        }

        #region + private static void ClearTimerTask()
        private static void ClearTimerTask()
        {
            try
            {
                if (_Timer != null)
                {
                    _Timer.Elapsed -= TimerTask_Elapsed;
                    _Timer.Enabled = false;
                    _Timer.Dispose();
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + private static void SetTimerTask()
        private static void SetTimerTask()
        {
            try
            {
                _Timer = new Timer();
                _Timer.Interval = TimeSpan.FromMinutes(AgentRegistry.AgentTimerMinute).TotalMilliseconds;
                _Timer.AutoReset = true;
                _Timer.Elapsed += TimerTask_Elapsed;

                _Timer.Enabled = true;                            
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

        // Timer Task Handler

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
                        new AgentHttpHelp().GetUsbWhitelist_Http();
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
