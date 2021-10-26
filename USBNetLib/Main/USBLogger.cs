using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBNetLib
{
    public class USBLogger
    {
        public static void Log(params string[] logs)
        {
            if (logs.Length > 0)
            {
                foreach (var l in logs)
                {
                    Console.WriteLine(l);
                }
            }
        }

        public static void Error(string error)
        {
            Console.WriteLine(error);
        }
    }
}
