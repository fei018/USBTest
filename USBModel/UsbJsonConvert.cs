using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using USBCommon;

namespace USBModel
{
    public class UsbJsonConvert
    {
        #region + public static PostComUsbInfo GetPostComputerUsbInfo(string json)
        public static PostComUsbHistory GetPostComputerUsbHistoryInfo(string postJson)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Converters = {
                        new AbstractJsonConverter<UserUsb, IUsbInfo>(),
                        new AbstractJsonConverter<UserComputer, IComputerInfo>(),
                        new AbstractJsonConverter<UsbHistory, IUsbHistory>()
                    }
                };

                var info = JsonConvert.DeserializeObject(postJson, typeof(PostComUsbHistory), settings) as PostComUsbHistory;
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
        public static UserUsb GetUserUsb(string usbJson)
        {
            try
            {
                var usb = JsonConvert.DeserializeObject<UserUsb>(usbJson);
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
