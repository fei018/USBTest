using System;
using System.Collections.Concurrent;
using System.IO;

namespace USBNetLib
{
    internal class PolicyTableHelp
    {
        private PolicyRule _policyRule;

        public PolicyTableHelp()
        {
            _policyRule = new PolicyRule();
        }

        #region + SetPolicyUSBList()
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
            PolicyTable.USBTable = list;
        }

        #endregion

        #region + public bool MatchPolicyTable(ref NotifyUSB notifyUsb) 
        private static object _locker = new object();

        public bool IsMatchPolicyTable(NotifyUSB notifyUsb)
        {
            lock (_locker)
            {
                if (!PolicyTable.IsAny())
                {
                    throw new Exception("PolicyTable.USBTable is Null.");
                }

                foreach (PolicyUSB pu in PolicyTable.USBTable)
                {
                    if (pu.IsMatchNotifyUSB(notifyUsb))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        #endregion

        #region + public void UpdateUSBList_Timer()
        public void UpdateUSBList_Timer()
        {

        }
        #endregion


    }
}
