using SqlSugar;
using USBCommon;

namespace USBModel
{
    public class UsbInfo : IUsbInfoHttp
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [SugarColumn(UniqueGroupNameList = new string[] { "usbid" })]
        public string UsbIdentity { get; set; }

        public bool IsRegistered { get; set; }

        public int Vid { get; set; }

        public int Pid { get; set; }

        public string SerialNumber { get; set; }


        [SugarColumn(IsNullable = true)]
        public string Manufacturer { get; set; }

        [SugarColumn(IsNullable = true)]
        public string DeviceDescription { get; set; }

        [SugarColumn(IsNullable = true)]
        public string Product { get; set; }


        // not mapping
        [SugarColumn(IsIgnore = true)]
        public string Pid_Hex => "PID_" + Pid.ToString("X").PadLeft(4, '0');

        [SugarColumn(IsIgnore = true)]
        public string Vid_Hex => "VID_" + Vid.ToString("X").PadLeft(4, '0');


        public override string ToString()
        {
            return $"\r\nManufacturer: {Manufacturer}\r\nProduct: { Product}\r\nVid: {Vid_Hex}\r\nPid: {Pid_Hex}\r\nSerialNumber: {SerialNumber}\r\n";
        }
    }
}
