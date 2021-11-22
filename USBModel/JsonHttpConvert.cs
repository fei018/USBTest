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
                        new AbstractJsonConverter<Tbl_UsbInfo, IUsbInfoHttp>(),
                        new AbstractJsonConverter<Tbl_UserComputer, IUserComputerHttp>(),
                        new AbstractJsonConverter<Tbl_UserUsbHistory, IUserUsbHistoryHttp>()
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
        public static Tbl_UserComputer Deserialize_UserComputer(string comJson)
        {
            try
            {
                var com = JsonConvert.DeserializeObject<Tbl_UserComputer>(comJson);
                return com;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region + public static UserUsb Deserialize_UsbInfo(string usbJson)
        public static Tbl_UsbInfo Deserialize_UsbInfo(string usbJson)
        {
            try
            {
                var usb = JsonConvert.DeserializeObject<Tbl_UsbInfo>(usbJson);
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
