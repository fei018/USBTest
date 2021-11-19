using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using USBCommon;

namespace USBModel
{
    public class JsonHttpConvert
    {
        #region + public static PostComUsbInfo GetPostComputerUsbInfo(string json)
        public static PostComUsbHistoryHttp GetPostComputerUsbHistoryInfo(string postJson)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Converters = {
                        new AbstractJsonConverter<UserUsbDetial, IUsbHttp>(),
                        new AbstractJsonConverter<UserComputer, IComputerHttp>(),
                        new AbstractJsonConverter<UsbHistory, IUsbHistoryHttp>()
                    }
                };

                var info = JsonConvert.DeserializeObject(postJson, typeof(PostComUsbHistoryHttp), settings) as PostComUsbHistoryHttp;
                return info;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public static UserComputer GetUserComputer(string comJson)
        public static UserComputer GetUserComputer(string comJson)
        {
            try
            {
                var com = JsonConvert.DeserializeObject<UserComputer>(comJson);
                return com;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region + public static UserUsb GetUserUsb(string usbJson)
        public static UserUsbDetial GetUserUsb(string usbJson)
        {
            try
            {
                var usb = JsonConvert.DeserializeObject<UserUsbDetial>(usbJson);
                return usb;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region + public static RequestAgentConfig GetRequestAgentConfig(string requestAgentConfigJson)
        public static RequestAgentData GetRequestAgentConfig(string requestAgentConfigJson)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Converters = {
                        new AbstractJsonConverter<UserComputer, IComputerHttp>(),
                        new AbstractJsonConverter<UserUsbFilterState, IAgentConfig>()
                    }
                };

                var info = JsonConvert.DeserializeObject(requestAgentConfigJson, typeof(RequestAgentData), settings) as RequestAgentData;
                return info;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
