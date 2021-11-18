using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using USBCommon;

namespace USBNotifyLib
{
    public class UsbHttpHelp
    {
        #region + public static void GetUsbFilterDb_Http()
        public static void GetUsbFilterDb_Http()
        {
            try
            {
                using (var http = new HttpClient())
                {
                    http.Timeout = TimeSpan.FromSeconds(10);
                    var response = http.GetAsync(UsbConfig.UsbFilterDbUrl).Result;
                    response.EnsureSuccessStatusCode();
                    string rp = response.Content.ReadAsStringAsync().Result;

                    UsbConfig.WriteFile_UsbFilterDb(rp);
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + public static void Set_GetUsbFilterDb_Http_Timer()
        private static Timer _updateUsbFilterDbTimer;
        public static void Set_GetUsbFilterDb_Http_Timer()
        {
            _updateUsbFilterDbTimer = new Timer();
            _updateUsbFilterDbTimer.Interval = UsbConfig.UpdateTimer;
            _updateUsbFilterDbTimer.AutoReset = false;
            _updateUsbFilterDbTimer.Elapsed += _getUsbFilterDbTimer_Elapsed;
            _updateUsbFilterDbTimer.Enabled = true;
        }

        private static void _getUsbFilterDbTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Task.Run(() =>
            {
                _updateUsbFilterDbTimer.Enabled = false;
                GetUsbFilterDb_Http();
                _updateUsbFilterDbTimer.Enabled = true;
            });
        }
        #endregion

        #region + public static void PostComputerInfo_Http()
        public static void PostComputerInfo_Http()
        {
            try
            {
                var com = new LocalComputer() as IComputerInfo;
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

        #region + public static void Set_PostComputerInfo_Http_Timer()
        private static Timer _postComInfo_Timer;
        public static void Set_PostComputerInfo_Http_Timer()
        {
            _postComInfo_Timer = new Timer();
            _postComInfo_Timer.Interval = UsbConfig.UpdateTimer;
            _postComInfo_Timer.AutoReset = false;
            _postComInfo_Timer.Elapsed += _postComInfo_Timer_Elapsed;
            _postComInfo_Timer.Enabled = true;
        }

        private static void _postComInfo_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Task.Run(() =>
            {
                _postComInfo_Timer.Enabled = false;
                PostComputerInfo_Http();
                _postComInfo_Timer.Enabled = true;
            });
        }
        #endregion

        #region + public void PostComputerUsbInfo_Http(char driveLetter)
        public void PostComputerUsbHistoryInfo_byVolume_Http(char driveLetter)
        {
            try
            {
                var com = new LocalComputer() as IComputerInfo;
                var usb = new UsbFilter().Find_NotifyUsb_Use_DriveLetter(driveLetter) as IUsbInfo;
                
                var his = new UsbHistory
                {
                    ComputerIdentity = com.ComputerIdentity,
                    UsbIdentity = usb.UsbIdentity,
                    PluginTime = DateTime.Now
                };

                var post = new PostComUsbHistory { ComputerInfo = com, UsbInfo = usb, UsbHistory = his };
                var postjosn = JsonConvert.SerializeObject(post);
             
                using (var http = new HttpClient())
                {
                    http.Timeout = TimeSpan.FromSeconds(10);
                    StringContent content = new StringContent(postjosn, Encoding.UTF8, "application/json");
                    var response = http.PostAsync(UsbConfig.PostComUsbHistoryInfoUrl, content).Result;
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + public void PostComputerUsbHistoryInfo_byDisk_Http(string diskPath)
        public void PostComputerUsbHistoryInfo_byDisk_Http(string diskPath)
        {
            try
            {
                var com = new LocalComputer() as IComputerInfo;
                var usb = new UsbFilter().Find_NotifyUsb_Use_DiskPath(diskPath) as IUsbInfo;

                var his = new UsbHistory
                {
                    ComputerIdentity = com.ComputerIdentity,
                    UsbIdentity = usb.UsbIdentity,
                    PluginTime = DateTime.Now
                };

                var post = new PostComUsbHistory { ComputerInfo = com, UsbInfo = usb, UsbHistory = his };
                var postjosn = JsonConvert.SerializeObject(post);

                using (var http = new HttpClient())
                {
                    http.Timeout = TimeSpan.FromSeconds(10);
                    StringContent content = new StringContent(postjosn, Encoding.UTF8, "application/json");
                    var response = http.PostAsync(UsbConfig.PostComUsbHistoryInfoUrl, content).Result;
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion
    }
}
