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
    public class AgentHttpHelp
    {
        #region + public static HttpClient CreateHttpClient()
        public static HttpClient CreateHttpClient()
        {
            var http = new HttpClient();
            http.Timeout = TimeSpan.FromSeconds(10);
            http.DefaultRequestHeaders.Add("AgentKey", AgentRegistry.AgentKey);
            return http;
        }
        #endregion

        #region + public static AgentHttpResponseResult DeserialAgentResult(string json)
        public static AgentHttpResponseResult DeserialAgentResult(string json)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Converters = {
                        new AbstractJsonConverter<AgentSetting, IAgentSetting>()
                    }
                };

                var agent = JsonConvert.DeserializeObject<AgentHttpResponseResult>(json, settings);
                return agent;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public void GetUsbFilterData_Http()
        public void GetUsbFilterData_Http()
        {
            try
            {
                using (var http = CreateHttpClient())
                {
                    http.Timeout = TimeSpan.FromSeconds(20);
                    
                    var response = http.GetAsync(AgentRegistry.UsbFilterDataUrl).Result;
                    if (!response.IsSuccessStatusCode) throw new Exception("Http Error StatusCode: " + response.StatusCode.ToString());

                    string json = response.Content.ReadAsStringAsync().Result;
                    var agentResult = DeserialAgentResult(json);
                    if (!agentResult.Succeed)
                    {
                        throw new Exception(agentResult.Msg);
                    }

                    UsbFilterDataHelp.Set_UsbFilterData_byHttp(agentResult.UsbFilterData);
                    UsbLogger.Log("Get UsbFilterData from Http Server done.");
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
                using (var http = CreateHttpClient())
                {
                    var response = http.GetAsync(AgentRegistry.AgentSettingUrl).Result;
                    if (!response.IsSuccessStatusCode) throw new Exception("Http Error StatusCode: " + response.StatusCode.ToString());

                    var json = response.Content.ReadAsStringAsync().Result;
                    var agentResult = DeserialAgentResult(json);

                    if (!agentResult.Succeed)
                    {
                        throw new Exception(agentResult.Msg);
                    }

                    var agentSetting = agentResult.AgentSetting;

                    AgentRegistry.AgentTimerMinute = agentSetting.AgentTimerMinute;
                    AgentRegistry.UsbFilterEnabled = agentSetting.UsbFilterEnabled;
                    AgentRegistry.UsbHistoryEnabled = agentSetting.UsbHistoryEnabled;
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

                using (var http = CreateHttpClient())
                {
                    StringContent content = new StringContent(comJson, Encoding.UTF8, MimeTypeMap.GetMimeType("json"));

                    var response =  http.PostAsync(AgentRegistry.PostPerComputerUrl, content).Result;
                    if (!response.IsSuccessStatusCode) throw new Exception("Http Error StatusCode: " + response.StatusCode.ToString());

                    var json = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<AgentHttpResponseResult>(json);
                    if (!result.Succeed)
                    {
                        throw new Exception(result.Msg);
                    }
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
                var usb = new UsbFilter().Find_UsbDisk_By_DiskPath(diskPath);

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

                using (var http = CreateHttpClient())
                {
                    StringContent content = new StringContent(usbHistoryJosn, Encoding.UTF8, "application/json");
                    var response = http.PostAsync(AgentRegistry.PostPerUsbHistoryUrl, content).Result;
                    if (!response.IsSuccessStatusCode) throw new Exception("Http Error StatusCode: " + response.StatusCode.ToString());

                    var json = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<AgentHttpResponseResult>(json);
                    if (!result.Succeed)
                    {
                        throw new Exception(result.Msg);
                    }
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
            }
        }
        #endregion

        #region + public void PostUsbRegisterRequest(UsbRegisterRequest usb)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="usb"></param>
        /// <exception cref="throw"></exception>
        public void PostUsbRegisterRequest(UsbRegRequest post)
        {
            try
            {
                if (post == null)
                {
                    throw new Exception("UsbRegisterRequest null reference.");
                }

                var usbJson = JsonConvert.SerializeObject(post);
                using (var http = CreateHttpClient())
                {
                    StringContent content = new StringContent(usbJson, Encoding.UTF8, MimeTypeMap.GetMimeType("json"));

                    var response = http.PostAsync(AgentRegistry.PostUsbRegRequestUrl, content).Result;

                    if (!response.IsSuccessStatusCode) throw new Exception("Http Error StatusCode: " + response.StatusCode.ToString());

                    var json = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<AgentHttpResponseResult>(json);
                    if (!result.Succeed)
                    {
                        throw new Exception(result.Msg);
                    }
                }
            }
            catch (Exception ex)
            {
                UsbLogger.Error(ex.Message);
                throw;
            }
        }
        #endregion

    }
}
