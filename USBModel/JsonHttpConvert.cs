using Newtonsoft.Json;
using System;
using USBCommon;

namespace USBModel
{
    public class JsonHttpConvert
    {
        #region + public static PostComUsbInfo Deserialize_PerUsbHistory(string json)
        public static Tbl_PerUsbHistory Deserialize_PerUsbHistory(string postJson)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Converters = {
                        new AbstractJsonConverter<Tbl_PerUsbHistory, IPerUsbHistory>()
                    }
                };

                var info = JsonConvert.DeserializeObject(postJson, typeof(Tbl_PerUsbHistory), settings) as Tbl_PerUsbHistory;
                return info;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public static UserComputer Deserialize_PerComputer(string comJson)
        public static Tbl_PerComputer Deserialize_PerComputer(string comJson)
        {
            try
            {
                var com = JsonConvert.DeserializeObject<Tbl_PerComputer>(comJson);
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
