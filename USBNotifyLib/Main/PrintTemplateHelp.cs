using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBNotifyLib
{
    public class PrintTemplateHelp
    {
        private static string _printBrmExe = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "spool\\tools\\PrintBrm.exe");

        #region MyRegion
        public void InstallPrinters()
        {
            try
            {
                // check PrintBrm.exe
                if (!File.Exists(_printBrmExe))
                {
                    throw new Exception("PrintBrm.exe not exist.");
                }

                // get template and check FilePath whether exist
                var template = new AgentHttpHelp().GetPrintTemplate_Http();
                //var tempFile = new FileInfo(template.FilePath.Trim());
                //if (tempFile.Exists)
                //{
                //    throw new Exception("PrintTemplate file not exist.\r\nPath: " + template.FilePath);
                //}

                // delete old network printers
                DeleteOldNetPrinters();

                // restore template printers
                RestoreTemplatePrinters(template.FilePath);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + private void DeleteOldNetPrinters()
        private void DeleteOldNetPrinters()
        {

        }
        #endregion

        #region + private void RestoreTemplatePrinters()
        private void RestoreTemplatePrinters(string file)
        {
            var command = $"{_printBrmExe} -R -NOACL -F {file}";

            var start = new ProcessStartInfo();
            start.FileName = "cmd.exe";
            start.UseShellExecute = false;
            start.WorkingDirectory = Environment.CurrentDirectory;
            start.CreateNoWindow = true;
            start.RedirectStandardError = true;
            start.RedirectStandardInput = true;
            start.RedirectStandardOutput = true;

            var ps = new Process();
            ps.StartInfo = start;
            ps.Start();

            ps.StandardInput.WriteLine(command);
            ps.StandardInput.WriteLine("exit");
            ps.WaitForExit();

            string error = ps.StandardError.ReadToEnd();
            if (string.IsNullOrWhiteSpace(error))
            {
                throw new Exception("RestoreTemplatePrinters Error: " + error);
            }
        }
        #endregion
    }
}
