using System;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using USBCommon;

namespace USBNetLib
{
    public class ComputerInfo : IComputerInfo
    {
        public string HostName { get; private set; }

        public string Domain { get; private set; }

        public string BiosSerial { get; private set; }

        public string IPAddress { get; private set; }

        public string MacAddress { get; private set; }


        public override string ToString()
        {
            return "HostName: " + HostName + "\r\n" +
                   "Domain: " + Domain + "\r\n" +
                   "BiosSerial: " + BiosSerial + "\r\n" +
                   "IPAddress: " + IPAddress + "\r\n" +
                   "MacAddress: " + MacAddress + "\r\n";
        }

        #region + public void GetInfo()
        public IComputerInfo GetInfo()
        {
            HostName = IPGlobalProperties.GetIPGlobalProperties().HostName;
            Domain = IPGlobalProperties.GetIPGlobalProperties().DomainName;
            GetIP();
            GetBiosSerial();
            return this;
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
            MacAddress = mac.ToString();

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
