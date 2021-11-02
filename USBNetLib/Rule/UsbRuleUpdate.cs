using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Timers;

namespace USBNetLib
{
    public class UsbRuleUpdate
    {
        #region + public static void UpdateRuleUSBTable()
        public static void UpdateRuleUSBTable()
        {
            try
            {
                using (var http = new HttpClient())
                {
                    http.Timeout = TimeSpan.FromSeconds(10);
                    var response = http.GetAsync(USBConfig.UpdateUrl).Result;
                    response.EnsureSuccessStatusCode();
                    string rp = response.Content.ReadAsStringAsync().Result;

                    USBConfig.Write_RuleUSbTable(rp);
                }
            }
            catch (Exception ex)
            {
                USBLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + public static void Set_UpdateTimer()
        private static Timer _updateTimer;
        public static void Set_UpdateTimer()
        {
            _updateTimer = new Timer();
            _updateTimer.Interval = USBConfig.UpdateTimer;
            _updateTimer.AutoReset = false;
            _updateTimer.Elapsed += _updateTimer_Elapsed;
            _updateTimer.Enabled = true;
        }

        private static void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _updateTimer.Enabled = false;
            UpdateRuleUSBTable();
            _updateTimer.Enabled = true;
        }
        #endregion
    }
}
