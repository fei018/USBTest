using System;
using System.IO;
using USBModel;

namespace USBAdminDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("start...");
            Console.WriteLine();
            try
            {
                var txt = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sql.txt");

                if (!File.Exists(txt))
                {
                    throw new Exception("connectionString file: \"sql.txt\" not exist.");
                }
               
                string conn = File.ReadAllText(txt)?.Trim();
                new USBAdminDatabaseHelp(conn).TryCreateDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("done...");
            Console.ReadLine();
        }
    }
}
