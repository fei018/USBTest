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

        public static bool HasUSBTable()
        {
            return USBTable != null && USBTable.Count > 0;
        }

        public void SetPolicyUSBList()
        {
            var list = new ConcurrentBag<PolicyUSB>();

            var file = USBConfig.PolicyTableFile;
            if (File.Exists(file))
            {
                var lines = File.ReadAllLines(file);

                if (lines.Length <= 0) return;

                foreach (var line in lines)
                {
                    if (line.Split(',').Length == 3)
                    {
                        var vid = UInt16.Parse(line.Split(',')[0]);
                        var pid = UInt16.Parse(line.Split(',')[1]);
                        var serial = line.Split(',')[2];

                        var usb = new PolicyUSB
                        {
                            Vid = vid,
                            Pid = pid,
                            SerialNumber = serial
                        };

                        list.Add(usb);
                    }
                }
            }
            USBTable = list;
        }



        public void UpdateUSBList_Timer()
        {

        }

        #region MatchPolicyTable 

        public bool MatchPolicyTable(ref NotifyUSB notifyUsb)
        {
            if (!HasUSBTable())
            {
                throw new Exception("Policy USB Table is Null or Empty.");
            }

            if (notifyUsb.HasVidPidSerial())
            {
                foreach (PolicyUSB pu in USBTable)
                {
                    if (pu.IsMatchNotifyUSB(notifyUsb))
                    {
                        return true;
                    }
                }           
            }
            return false;
        }
        #endregion
    }
}
