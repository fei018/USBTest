using NativeUsbLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace USBNetLib
{
    internal class PolicyTable
    {
        public static ConcurrentBag<PolicyUSB> USBTable;

        public static bool IsAny()
        {
            return USBTable != null && USBTable.Count > 0;
        }
    }
}
