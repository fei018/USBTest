using System;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using USBCommon;

namespace USBNotifyLib
{
    public class LocalComputer : IComputerHttp
    {
        /// <summary>
        /// new() 自動set好屬性值
        /// </summary>
        public LocalComputer()
        {
            SetInfo();
        }

        public string HostName { get;  set; }

        public string Domain { get;  set; }

        public string BiosSerial { get;  set; }

        public string IPAddress { get;  set; }

        public string MacAddress { get;  set; }

        public string ComputerIdentity => (BiosSerial + MacAddress)?.ToLower();

        public override string ToString()
        {
            return "HostName: " + HostName + "\r\n" +
                   "Domain: " + Domain + "\r\n" +
                   "BiosSerial: " + BiosSerial + "\r\n" +
                   "IPAddress: " + IPAddress + "\r\n" +
                   "MacAddress: " + MacAddress + "\r\n";
        }

        #region + private void SetInfo()
        private void SetInfo()
        {
            try
            {
                HostName = IPGlobalProperties.GetIPGlobalProperties().HostName;
                Domain = IPGlobalProperties.GetIPGlobalProperties().DomainName;
                SetIPMacAddress();
                SetBiosSerial();
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region + private void SetIPMacAddress()
        private void SetIPMacAddress()
        {
            var nic = NetworkInterface.GetAllNetworkInterfaces()
                                    .Where(n => n.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                                    .Where(n => n.OperationalStatus == OperationalStatus.Up).FirstOrDefault();

            //如果 wire nic 搵唔到, 嘗試搵 wireless nic
            if (nic == null)
            {
                nic = NetworkInterface.GetAllNetworkInterfaces()
                                    .Where(n => n.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                                    .Where(n => n.OperationalStatus == OperationalStatus.Up).FirstOrDefault();
            }

            if (nic == null) return;

            // set IP Address
            IPAddress = nic.GetIPProperties().UnicastAddresses
                            .Where(n => n.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            .First().Address.ToString();

            // Set MAC Address
            StringBuilder mac = new StringBuilder();
            byte[] bytes = nic.GetPhysicalAddress().GetAddressBytes();
            for (int i = 0; i < bytes.Length; i++)
            {
                // Display the physical address in hexadecimal.
                mac.Append(bytes[i].ToString("X2"));
                // Insert a hyphen after each byte, unless we are at the end of the address.
                if (i != bytes.Length - 1) mac.Append("-");
            }
            MacAddress = mac.ToString();            
        }
        #endregion

        #region + private void SetBiosSerial()
        private void SetBiosSerial()
        {
            using (ManagementObjectSearcher ComSerial = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS"))
            {
                using (var wmi = ComSerial.Get())
                {
                    foreach (var b in wmi)
                    {
                        BiosSerial = Convert.ToString(b["SerialNumber"])?.Trim();
                    }
                }
            }

            if (string.IsNullOrEmpty(BiosSerial))
            {
                using (ManagementObjectSearcher ComSerial = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard"))
                {
                    using (var wmi = ComSerial.Get())
                    {
                        foreach (var b in wmi)
                        {
                            BiosSerial = Convert.ToString(b["SerialNumber"])?.Trim();
                        }
                    }
                }
            }
        }
        #endregion
    }
}
