using SqlSugar;
using System;

namespace USBModel
{
    public class UserUsbRegisterRForm
    {
        [SugarColumn(IsIdentity =true,IsPrimaryKey =true)]
        public int Id { get; set; }

        [SugarColumn(IsNullable = true)]
        public DateTime RequestTime { get; set; }

        public string RequestComputerIdentity { get; set; }

        public string HHUserId { get; set; }
    }
}
