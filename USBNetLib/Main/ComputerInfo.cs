﻿using System;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using USBCommon;

namespace USBNetLib
{
    public class ComputerInfo: IComputerInfo
    {
        public string HostName { get; private set; }

        public string Domain { get; private set; }

        public string BiosSerial { get; private set; }

        public string IPAddress { get; private set; }

        public string MACAddress { get; private set; }

        public ComputerInfo()
        {
            GetInfo();
        }

        #region + private void GetInfo()
        private void GetInfo()
        {
            HostName = IPGlobalProperties.GetIPGlobalProperties().HostName;
            Domain = IPGlobalProperties.GetIPGlobalProperties().DomainName;
            GetIP();
            GetBiosSerial();
        }

        private void GetIP()
        {
            var nic = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                .Where(n => n.OperationalStatus == OperationalStatus.Up).FirstOrDefault();

            if (nic == null) return;

            StringBuilder mac = new StringBuilder();
            byte[] bytes = nic.GetPhysicalAddress().GetAddressBytes();
            for (int i = 0; i < bytes.Length; i++)
            {
                // Display the physical address in hexadecimal.
                mac.Append(bytes[i].ToString("X2"));
                // Insert a hyphen after each byte, unless we are at the end of the
                // address.
                if (i != bytes.Length - 1) mac.Append("-");
            }
            MACAddress = mac.ToString();

            IPAddress = nic.GetIPProperties().UnicastAddresses.First().Address.ToString();
        }
        #endregion

        #region + private void GetBiosSerial()
        private void GetBiosSerial()
        {
            using (ManagementObjectSearcher ComSerial = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS"))
            {
                using (var wmi = ComSerial.Get())
                {
                    foreach (var b in wmi)
                    {
                        BiosSerial = Convert.ToString(b["SerialNumber"]);
                    }
                }
            }
        }
        #endregion
    }
}