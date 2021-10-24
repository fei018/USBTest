using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBNetLib
{
    internal class PolicyTable
    {
        public static List<PolicyUSB> USBList;

        public void SetPolicyUSBList()
        {
            var list = new List<PolicyUSB>();

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
                            VID = vid,
                            PID = pid,
                            SerialNumber = serial
                        };

                        list.Add(usb);
                    }
                }
            }
            USBList = list;
        }

        public void UpdateUSBList_Timer()
        {

        }
    }
}
