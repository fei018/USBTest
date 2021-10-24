using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace USBTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                new USBHelp().Test2();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }

    }
}
