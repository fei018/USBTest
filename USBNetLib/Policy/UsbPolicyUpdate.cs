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
    public class UsbPolicyUpdate
    {
        #region + public static void HttpGetPolicyUSBTable()
        public static void HttpGetPolicyUSBTable()
        {
            try
            {
                using (var http = new HttpClient())
                {
                    http.Timeout = TimeSpan.FromSeconds(10);
                    var response = http.GetAsync(USBConfig.GetPolicyUrl).Result;
                    response.EnsureSuccessStatusCode();
                    string rp = response.Content.ReadAsStringAsync().Result;

                    USBConfig.Write_PolicyUSbTable(rp);
                }
            }
            catch (Exception ex)
            {
                USBLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + public static void Set_HttpGetPolicy_Timer()
        private static Timer _updatePolicyTimer;
        public static void Set_HttpGetPolicy_Timer()
        {
            _updatePolicyTimer = new Timer();
            _updatePolicyTimer.Interval = USBConfig.UpdateTimer;
            _updatePolicyTimer.AutoReset = false;
            _updatePolicyTimer.Elapsed += _updatePolicyTimer_Elapsed;
            _updatePolicyTimer.Enabled = true;
        }

        private static void _updatePolicyTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Task.Run(() =>
            {
                _updatePolicyTimer.Enabled = false;
                HttpGetPolicyUSBTable();
                _updatePolicyTimer.Enabled = true;
            });
        }
        #endregion

        #region + public static void HttpPostComputerInfo()
        public static void HttpPostComputerInfo()
        {
            try
            {
                var com = new ComputerInfo().GetInfo();
                var comJson = JsonConvert.SerializeObject(com);

                using (var http = new HttpClient())
                {
                    http.Timeout = TimeSpan.FromSeconds(10);
                    StringContent content = new StringContent(comJson, Encoding.UTF8, "application/json");
                    var response = http.PostAsync(USBConfig.PostComputerInfoUrl, content).Result;
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                USBLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + public static void Set_HttpPostComputerInfo_Timer()
        private static Timer _postComInfoTimer;
        public static void Set_HttpPostComputerInfo_Timer()
        {
            _postComInfoTimer = new Timer();
            _postComInfoTimer.Interval = USBConfig.UpdateTimer;
            _postComInfoTimer.AutoReset = false;
            _postComInfoTimer.Elapsed += _postComInfoTimer_Elapsed;
            _postComInfoTimer.Enabled = true;
        }

        private static void _postComInfoTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Task.Run(() => {
                _postComInfoTimer.Enabled = false;
                HttpPostComputerInfo();
                _postComInfoTimer.Enabled = true;
            });
        }
        #endregion
    }
}
