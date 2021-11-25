﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBCommon
{
    public interface IUserUsbHistory:IUsbInfo
    {

        string ComputerIdentity { get; set; }

        DateTime PluginTime { get; set; }
    }
}