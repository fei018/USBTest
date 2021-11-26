using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace USBNotifyLib
{
    public class PerComputerHelp
    {
        #region public static string GetComputerIdentity()
        public static string GetComputerIdentity()
        {
            try
            {
                var com = new PerComputerHelp().GetInfo();
                if (string.IsNullOrWhiteSpace(com.ComputerIdentity))
                {
                    throw new Exception("ComputerIdentity is null or empty.");
                }
                return com.ComputerIdentity;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public static PerComputer GetPerComputer()
        public static PerComputer GetPerComputer()
        {
            try
            {
                return new PerComputerHelp().GetInfo();

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region + private PerComputer GetInfo()
        private PerComputer GetInfo()
        {
            try
            {
                PerComputer userComputer = new PerComputer();
                userComputer.AgentVersion = UsbRegistry.AgentVersion;
                userComputer.UsbFilterEnabled = UsbRegistry.UsbFilterEnabled;
                userComputer.UsbHistoryEnabled = UsbRegistry.UsbHistoryEnabled;
                userComputer.HostName = IPGlobalProperties.GetIPGlobalProperties().HostName;
                userComputer.Domain = IPGlobalProperties.GetIPGlobalProperties().DomainName;
                SetIPMacAddress(userComputer);
                SetBiosSerial(userComputer);
                return userComputer;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + private void SetIPMacAddress()
        private void SetIPMacAddress(PerComputer userComputer)
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
            userComputer.IPAddress = nic.GetIPProperties().UnicastAddresses
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
            userComputer.MacAddress = mac.ToString();
        }
        #endregion

        #region + private void SetBiosSerial()
        private void SetBiosSerial(PerComputer userComputer)
        {
            using (ManagementObjectSearcher ComSerial = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS"))
            {
                using (var wmi = ComSerial.Get())
                {
                    foreach (var b in wmi)
                    {
                        userComputer.BiosSerial = Convert.ToString(b["SerialNumber"])?.Trim();
                    }
                }
            }

            if (string.IsNullOrEmpty(userComputer.BiosSerial))
            {
                using (ManagementObjectSearcher ComSerial = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard"))
                {
                    using (var wmi = ComSerial.Get())
                    {
                        foreach (var b in wmi)
                        {
                            userComputer.BiosSerial = Convert.ToString(b["SerialNumber"])?.Trim();
                        }
                    }
                }
            }
        }
        #endregion
    }
}
