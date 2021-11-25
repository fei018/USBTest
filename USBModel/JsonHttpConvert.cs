using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using USBCommon;

namespace USBModel
{
    public class JsonHttpConvert
    {
        #region + public static PostComUsbInfo Deserialize_UserUsbHistory(string json)
        public static Tbl_UserUsbHistory Deserialize_UserUsbHistory(string postJson)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Converters = {
                        new AbstractJsonConverter<Tbl_UserUsbHistory, IUserUsbHistory>()
                    }
                };

                var info = JsonConvert.DeserializeObject(postJson, typeof(Tbl_UserUsbHistory), settings) as Tbl_UserUsbHistory;
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

        #region + public static UserUsb Deserialize_UsbRegistered(string usbJson)
        public static Tbl_UsbRegistered Deserialize_UsbRegistered(string usbJson)
        {
            try
            {
                var usb = JsonConvert.DeserializeObject<Tbl_UsbRegistered>(usbJson);
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
