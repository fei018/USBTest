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
            new SetupHelp().Install();

            Environment.Exit(0);
        }
    }
}
