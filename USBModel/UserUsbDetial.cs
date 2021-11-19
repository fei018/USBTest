using SqlSugar;
using System;
using USBCommon;

namespace USBModel
{
    public class UserUsbDetial : UsbInfo, IUsbHttp
    {       
        public string Pid_Hex => "PID_" + Pid.ToString("X").PadLeft(4, '0');

        public string Vid_Hex => "VID_" + Vid.ToString("X").PadLeft(4, '0');


        public override string ToString()
        {
            return $"\r\nManufacturer: {Manufacturer}\r\nProduct: { Product}\r\nVid: {Vid_Hex}\r\nPid: {Pid_Hex}\r\nSerialNumber: {SerialNumber}\r\n";
        }
    }
}
