using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Timers;
using Newtonsoft;
using USBCommon;
using Newtonsoft.Json;

namespace USBNetLib
{
    public class UsbManager
    {
        #region + public static void UpdateUsbFilterTable_Http()
        public static void UpdateUsbFilterTable_Http()
        {
            try
            {
                using (var http = new HttpClient())
                {
                    http.Timeout = TimeSpan.FromSeconds(10);
                    var response = http.GetAsync(UsbConfig.GetUsbFilterUrl).Result;
                    response.EnsureSuccessStatusCode();
                    string rp = response.Content.ReadAsStringAsync().Result;

                    UsbConfig.Write_FilterUSbTable(rp);
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + public static void Set_UpdateUsbFilterTable_Http_Timer()
        private static Timer _updateUsbFilterTableTimer;
        public static void Set_UpdateUsbFilterTable_Http_Timer()
        {
            _updateUsbFilterTableTimer = new Timer();
            _updateUsbFilterTableTimer.Interval = UsbConfig.UpdateTimer;
            _updateUsbFilterTableTimer.AutoReset = false;
            _updateUsbFilterTableTimer.Elapsed += _updateUsbFilterTableTimer_Elapsed;
            _updateUsbFilterTableTimer.Enabled = true;
        }

        private static void _updateUsbFilterTableTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Task.Run(() =>
            {
                _updateUsbFilterTableTimer.Enabled = false;
                UpdateUsbFilterTable_Http();
                _updateUsbFilterTableTimer.Enabled = true;
            });
        }
        #endregion

        #region + public static void UpdateComputerInfo_Http()
        public static void UpdateComputerInfo_Http()
        {
            try
            {
                var com = new ComputerInfo().GetInfo();
                var comJson = JsonConvert.SerializeObject(com);

                using (var http = new HttpClient())
                {
                    http.Timeout = TimeSpan.FromSeconds(10);
                    StringContent content = new StringContent(comJson, Encoding.UTF8, "application/json");
                    var response = http.PostAsync(UsbConfig.PostComputerInfoUrl, content).Result;
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + public static void Set_HttpPostComputerInfo_Timer()
        private static Timer _updateComInfo_Timer;
        public static void Set_UpdateComputerInfo_Http_Timer()
        {
            _updateComInfo_Timer = new Timer();
            _updateComInfo_Timer.Interval = UsbConfig.UpdateTimer;
            _updateComInfo_Timer.AutoReset = false;
            _updateComInfo_Timer.Elapsed += _updateComInfo_Timer_Elapsed;
            _updateComInfo_Timer.Enabled = true;
        }

        private static void _updateComInfo_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Task.Run(() => {
                _updateComInfo_Timer.Enabled = false;
                UpdateComputerInfo_Http();
                _updateComInfo_Timer.Enabled = true;
            });
        }
        #endregion
    }
}
