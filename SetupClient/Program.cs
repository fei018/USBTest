using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetupClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Install Start ...");
                Console.WriteLine();

                new SetupHelp().Install();

                Console.WriteLine("Install Done !!!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

            Environment.Exit(0);
        }
    }
}
