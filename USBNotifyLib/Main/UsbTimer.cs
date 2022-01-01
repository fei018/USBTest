using System.Threading.Tasks;
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
            SetTimer_CheckAndUpdateAgent();
        }


        #region + private static void SetTimer_GetAgentSetting()
        private static void SetTimer_GetAgentSetting()
        {
            try
            {
                var agentSettingTimer = new Timer();
                agentSettingTimer.Interval = TimeSpan.FromMinutes(AgentRegistry.AgentTimerMinute).TotalMilliseconds;
                agentSettingTimer.AutoReset = true;
                agentSettingTimer.Elapsed += AgentSettingTimer_Elapsed;

                agentSettingTimer.Enabled = true;                            
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }

        private static void AgentSettingTimer_Elapsed(object sender, ElapsedEventArgs e)
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
        }
        #endregion

        #region + private static void SetTimer_PostUserComputer()
        private static void SetTimer_PostUserComputer()
        {
            try
            {
                var postPerComputerTimer = new Timer();
                postPerComputerTimer.Interval = TimeSpan.FromMinutes(AgentRegistry.AgentTimerMinute).TotalMilliseconds;
                postPerComputerTimer.AutoReset = true;
                postPerComputerTimer.Elapsed += PostPerComputerTimer_Elapsed;

                postPerComputerTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }

        private static void PostPerComputerTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                new AgentHttpHelp().PostPerComputer_Http();
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

        #region private static void SetTimer_CheckAndUpdateAgent()
        private static void SetTimer_CheckAndUpdateAgent()
        {
            try
            {
                var checkAndUpdateAgentTimer = new Timer();
                checkAndUpdateAgentTimer.Interval = TimeSpan.FromMinutes(AgentRegistry.AgentTimerMinute).TotalMilliseconds;
                checkAndUpdateAgentTimer.AutoReset = true;
                checkAndUpdateAgentTimer.Elapsed += CheckAndUpdateAgentTimer_Elapsed;

                checkAndUpdateAgentTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }

        private static void CheckAndUpdateAgentTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            AgentUpdate.CheckAndUpdate();
        }
        #endregion
    }
}
