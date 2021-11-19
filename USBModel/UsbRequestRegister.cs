using SqlSugar;
using System;

namespace USBModel
{
    public class UsbRequestRegister
    {
        [SugarColumn(IsNullable = true)]
        public DateTime RequestRegisterTime { get; set; }

        public string PostComputerIdentity { get; set; }
    }
}
