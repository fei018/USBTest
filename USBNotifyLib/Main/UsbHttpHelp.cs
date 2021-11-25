using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using USBCommon;
using MimeTypes;

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

                    var comIdentity = UserComputerHelp.GetComputerIdentity();
                    Debug.WriteLine(comIdentity);

                    var url = UsbRegistry.UsbFilterDbUrl + "?computerIdentity=" + comIdentity;
                    Debug.WriteLine(url);
                    //Debugger.Break();
                    var response = http.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string rp = response.Content.ReadAsStringAsync().Result;
                        GetUsbFilterDbHttp db = JsonConvert.DeserializeObject<GetUsbFilterDbHttp>(rp);

                        UsbFilterDbHelp.Set_UsbFilterDb_byHttp(db);
                    }
                    else
                    {
                        throw new Exception(response.StatusCode.ToString());
                    }                    
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + public static void GetAgentSetting_Http()
        public static void GetAgentSetting_Http()
        {
            try
            {
                using (var http = new HttpClient())
                {
                    http.Timeout = TimeSpan.FromSeconds(10);

                    var url = UsbRegistry.AgentSettingUrl;
                    Debug.WriteLine(url);
                    
                    var response = http.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var json = response.Content.ReadAsStringAsync().Result;

                        var setting = JsonConvert.DeserializeObject(json, typeof(AgentSetting)) as AgentSetting;

                        UsbRegistry.AgentTimerMinute = setting.AgentTimerMinute;
                        AgentUpdate.Check(setting.Version);
                    }
                    else
                    {
                        throw new Exception(response.StatusCode.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + public static void PostUserComputer_Http()
        public static void PostUserComputer_Http()
        {
            try
            {
                var com = UserComputerHelp.GetUserComputer() as IUserComputerHttp;
                var comJson = JsonConvert.SerializeObject(com);

                using (var http = new HttpClient())
                {
                    http.Timeout = TimeSpan.FromSeconds(10);
                    StringContent content = new StringContent(comJson, Encoding.UTF8, MimeTypeMap.GetMimeType("json"));

                    var response = http.PostAsync(UsbRegistry.PostUserComputerUrl, content).Result;

                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + public void PostUserUsbHistory_byVolume_Http(char driveLetter)
        public void PostUserUsbHistory_byVolume_Http(char driveLetter)
        {
            try
            {
                var com = UserComputerHelp.GetUserComputer() as IUserComputerHttp;
                var usb = new UsbFilter().Find_NotifyUsb_Use_DriveLetter(driveLetter) as IUsbInfoHttp;
                
                var his = new UsbHistory
                {
                    ComputerIdentity = com.ComputerIdentity,
                    UsbIdentity = usb.UsbIdentity,
                    PluginTime = DateTime.Now
                };

                var post = new PostUserUsbHistoryHttp { UserComputer = com, UsbInfo = usb, UserUsbHistory = his };
                var postjosn = JsonConvert.SerializeObject(post);
             
                using (var http = new HttpClient())
                {
                    http.Timeout = TimeSpan.FromSeconds(10);
                    StringContent content = new StringContent(postjosn, Encoding.UTF8, "application/json");
                    var response = http.PostAsync(UsbRegistry.PostUserUsbHistoryUrl, content).Result;
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + public void PostUserUsbHistory_byDisk_Http(string diskPath)
        public void PostUserUsbHistory_byDisk_Http(string diskPath)
        {
            try
            {
                var com = UserComputerHelp.GetUserComputer() as IUserComputerHttp;
                var usb = new UsbFilter().Find_NotifyUsb_Use_DiskPath(diskPath) as IUsbInfoHttp;

                var his = new UsbHistory
                {
                    ComputerIdentity = com.ComputerIdentity,
                    UsbIdentity = usb.UsbIdentity,
                    PluginTime = DateTime.Now
                };

                var post = new PostUserUsbHistoryHttp { UserComputer = com, UsbInfo = usb, UserUsbHistory = his };
                var postjosn = JsonConvert.SerializeObject(post);

                using (var http = new HttpClient())
                {
                    http.Timeout = TimeSpan.FromSeconds(10);
                    StringContent content = new StringContent(postjosn, Encoding.UTF8, "application/json");
                    var response = http.PostAsync(UsbRegistry.PostUserUsbHistoryUrl, content).Result;
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
