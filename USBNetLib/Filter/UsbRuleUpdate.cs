using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;

namespace USBNetLib.Filter
{
    public class UsbRuleUpdate
    {
        #region + public void UpdateRuleUSBTable()
        public void UpdateRuleUSBTable()
        {
            try
            {
                using (var http = new HttpClient())
                {
                    http.Timeout = TimeSpan.FromSeconds(10);
                    var response = http.GetAsync(USBConfig.UpdateUrl).Result;
                    response.EnsureSuccessStatusCode();
                    string rp = response.Content.ReadAsStringAsync().Result;

                    File.WriteAllText(USBConfig.RuleUSBTablePath, rp);
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

    }
}
