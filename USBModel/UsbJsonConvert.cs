using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using USBCommon;

namespace USBModel
{
    public class UsbJsonConvert
    {
        public static PostComUsb GetPostComputerUsbInfo(string json)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Converters = {
                        new AbstractJsonConverter<RegisteredUsb, IUsbInfo>(),
                        new AbstractJsonConverter<ComputerInfo, IComputerInfo>()
                    }
                };

                var info = JsonConvert.DeserializeObject(json, typeof(PostComUsb), settings) as PostComUsb;
                return info;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
