using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using USBCommon;

namespace USBNotifyLib
{
    public class UsbConfig
    {

        private static readonly string _baseDir = AppDomain.CurrentDomain.BaseDirectory;


        public static string LogPath => Path.Combine(_baseDir, "log.txt");

        public static string ErrorPath => Path.Combine(_baseDir, "error.txt");

        public static string UsbFilterDbFile => Path.Combine(_baseDir, "usbFilterDb.dat");

        // Registry

        public static bool UserUsbFilterEnabled { get;set;}

        public static string AgentDataUrl { get; set; }

        public static int AgentTimerMinute { get; set; }

        public static string PostUserComputerUrl { get; set; }

        public static string PostUserUsbHistoryUrl { get; set; }


       
    }
}
