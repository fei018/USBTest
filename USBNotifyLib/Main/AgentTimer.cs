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
            try
            {
                // 上載 本機資訊
                new AgentHttpHelp().PostPerComputer_Http();

                // update agent setting
                new AgentHttpHelp().GetAgentSetting_Http();

                // update UsbWhitelist
                new AgentHttpHelp().GetUsbWhitelist_Http();

                ClearTimerTask();
                SetTimerTask();
            }
            catch (Exception ex)
            {
                AgentLogger.Error(ex.Message);
            }
        }

        #region + private static void ClearTimerTask()
        public static void ClearTimerTask()
        {
            try
            {
                if (_Timer != null)
                {
                    _Timer.Elapsed -= TimerTask_Elapsed;
                    _Timer.Stop();
                    _Timer.Dispose();
                }
            }
            catch (Exception ex)
            {
                AgentLogger.Error(ex.Message);
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

                _Timer.Start();                            
            }
            catch (Exception ex)
            {
                AgentLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + private static void TimerTask_Elapsed(object sender, ElapsedEventArgs e)
        private static void TimerTask_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _Timer.Stop();

                new AgentHttpHelp().PostPerComputer_Http();
                new AgentHttpHelp().GetAgentSetting_Http();

                if (AgentRegistry.UsbFilterEnabled)
                {
                    new AgentHttpHelp().GetUsbWhitelist_Http();
                    new UsbFilter().Filter_Scan_All_USB_Disk();
                }

                AgentUpdate.CheckAndUpdate();
            }
            catch (Exception ex)
            {
                AgentLogger.Error(ex.Message);
            }
            finally
            {
                _Timer.Start();
            }
        }
        #endregion
    }
}
