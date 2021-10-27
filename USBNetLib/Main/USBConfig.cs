using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace USBNetLib
{
    public class USBConfig
    {
        private static readonly string _baseDir = AppDomain.CurrentDomain.BaseDirectory;

        public static string FilterUSBTablePath => Path.Combine(_baseDir, "policytable.txt");
    }
}
