using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using USBCommon;

namespace USBNotifyLib
{
    public class UsbHttpHelp
    {
        #region + public void GetUsbFilterDb_Http()
        public void GetUsbFilterDb_Http()
        {
            try
            {
                using (var http = new HttpClient())
                {
                    http.Timeout = TimeSpan.FromSeconds(20);

                    var comIdentity = PerComputerHelp.GetComputerIdentity();

                    var url = UsbRegistry.UsbFilterDbUrl + "?computerIdentity=" + comIdentity;
                    
                    var response = http.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        UsbFilterDbHttp db = JsonConvert.DeserializeObject<UsbFilterDbHttp>(json);

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

        #region + public void GetAgentSetting_Http()
        public void GetAgentSetting_Http()
        {
            try
            {
                using (var http = new HttpClient())
                {
                    http.Timeout = TimeSpan.FromSeconds(10);

                    var comIdentity = PerComputerHelp.GetComputerIdentity();

                    var url = UsbRegistry.AgentSettingUrl + "?computerIdentity=" + comIdentity;

                    var response = http.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var json = response.Content.ReadAsStringAsync().Result;

                        var setting = JsonConvert.DeserializeObject<AgentSetting>(json);

                        UsbRegistry.UsbFilterEnabled = setting.UsbFilterEnabled;
                        AgentManager.IsUsbFilterEnable = setting.UsbFilterEnabled;

                        UsbRegistry.UsbHistoryEnabled = setting.UsbHistoryEnabled;
                        AgentManager.IsUsbHistoryEnable = setting.UsbHistoryEnabled;
                        if (setting.UsbFilterEnabled)
                        {
                            GetUsbFilterDb_Http();
                        }

                        UsbRegistry.AgentTimerMinute = setting.AgentTimerMinute;
                        AgentUpdate.Check(setting.AgentVersion);
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

        #region + public void PostPerComputer_Http()
        public void PostPerComputer_Http()
        {
            try
            {
                var com = PerComputerHelp.GetPerComputer() as IPerComputer;
                var comJson = JsonConvert.SerializeObject(com);

                using (var http = new HttpClient())
                {
                    http.Timeout = TimeSpan.FromSeconds(10);
                    StringContent content = new StringContent(comJson, Encoding.UTF8, MimeTypeMap.GetMimeType("json"));

                    var response =  http.PostAsync(UsbRegistry.PostPerComputerUrl, content).Result;

                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + public void PostPerUsbHistory_byDisk_Http(string diskPath)
        public void PostPerUsbHistory_byDisk_Http(string diskPath)
        {
            try
            {
                //Debugger.Break();
                var comIdentity = PerComputerHelp.GetComputerIdentity();
                var usb = new UsbFilter().Find_NotifyUsb_Use_DiskPath(diskPath);

                IPerUsbHistory usbHistory = new PerUsbHistory
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
                    var response = http.PostAsync(UsbRegistry.PostPerUsbHistoryUrl, content).Result;
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
