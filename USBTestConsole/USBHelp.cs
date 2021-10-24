using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using USBNetLib;

namespace USBTestConsole
{
    public class USBHelp
    {

        public void Test2()
        {
            USBManager usb = new USBManager();
            usb.Start();

            Console.ReadLine();
            usb.Close();
            

        }
    }
}
