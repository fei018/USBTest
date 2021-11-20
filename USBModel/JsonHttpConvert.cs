using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using USBCommon;

namespace USBModel
{
    public class JsonHttpConvert
    {
        #region + public static PostComUsbInfo Deserialize_PostUserUsbHistory(string json)
        public static PostUserUsbHistoryHttp Deserialize_PostUserUsbHistory(string postJson)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Converters = {
                        new AbstractJsonConverter<UsbInfo, IUsbInfoHttp>(),
                        new AbstractJsonConverter<UserComputer, IUserComputerHttp>(),
                        new AbstractJsonConverter<UserUsbHistory, IUserUsbHistoryHttp>()
                    }
                };

                var info = JsonConvert.DeserializeObject(postJson, typeof(PostUserUsbHistoryHttp), settings) as PostUserUsbHistoryHttp;
                return info;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public static UserComputer Deserialize_UserComputer(string comJson)
        public static UserComputer Deserialize_UserComputer(string comJson)
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

        #region + public static UserUsb Deserialize_UsbInfo(string usbJson)
        public static UsbInfo Deserialize_UsbInfo(string usbJson)
        {
            try
            {
                var usb = JsonConvert.DeserializeObject<UsbInfo>(usbJson);
                return usb;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

    }
}
