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
                    
                    var response = http.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        GetUsbFilterDbHttp db = JsonConvert.DeserializeObject<GetUsbFilterDbHttp>(json);

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
                var com = UserComputerHelp.GetUserComputer() as IUserComputer;
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

        #region + public void PostUserUsbHistory_byDisk_Http(string diskPath)
        public void PostUserUsbHistory_byDisk_Http(string diskPath)
        {
            try
            {
                Debugger.Break();
                var comIdentity = UserComputerHelp.GetComputerIdentity();
                var usb = new UsbFilter().Find_NotifyUsb_Use_DiskPath(diskPath);

                IUserUsbHistory usbHistory = new UserUsbHistory
                {
                    ComputerIdentity = comIdentity,
                    DeviceDescription = usb.DeviceDescription,
                    Manufacturer = usb.Manufacturer,
                    Pid = usb.Pid,
                    Product = usb.Product,
                    SerialNumber = usb.SerialNumber,
                    Vid = usb.Vid,
                    PluginTime = DateTime.Now
                };
                
                var usbHistoryJosn = JsonConvert.SerializeObject(usbHistory);

                using (var http = new HttpClient())
                {
                    http.Timeout = TimeSpan.FromSeconds(10);
                    StringContent content = new StringContent(usbHistoryJosn, Encoding.UTF8, "application/json");
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


        #region + public void PostUserUsbHistory_byVolume_Http(char driveLetter)
        //public void PostUserUsbHistory_byVolume_Http(char driveLetter)
        //{
        //    try
        //    {
        //        var comIdentity = UserComputerHelp.GetComputerIdentity();
        //        var usb = new UsbFilter().Find_NotifyUsb_Use_DriveLetter(driveLetter);

        //        IUserUsbHistory usbHistory = new UserUsbHistory
        //        {
        //            ComputerIdentity = comIdentity,
        //            DeviceDescription = usb.DeviceDescription,
        //            Manufacturer = usb.Manufacturer,
        //            Pid = usb.Pid,
        //            Product = usb.Product,
        //            SerialNumber = usb.SerialNumber,
        //            Vid = usb.Vid,
        //            PluginTime = DateTime.Now
        //        };

        //        var usbHistoryJosn = JsonConvert.SerializeObject(usbHistory);

        //        using (var http = new HttpClient())
        //        {
        //            http.Timeout = TimeSpan.FromSeconds(10);
        //            StringContent content = new StringContent(usbHistoryJosn, Encoding.UTF8, "application/json");
        //            var response = http.PostAsync(UsbRegistry.PostUserUsbHistoryUrl, content).Result;
        //            response.EnsureSuccessStatusCode();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        UsbLogger.Error(ex.Message);
        //    }
        //}
        #endregion
    }
}
