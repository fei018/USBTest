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
        private string _printBrmExe;

        public PrintTemplateHelp()
        {
            _printBrmExe = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "spool\\tools\\PrintBrm.exe");
        }

        #region + public void Start()
        /// <summary>
        /// 
        /// </summary>
        /// <returns>StandardOutput</returns>
        /// <exception cref="StandardError"></exception>
        public string Start()
        {
            try
            {
                // check PrintBrm.exe
                if (!File.Exists(_printBrmExe))
                {
                    throw new Exception("PrintBrm.exe not exist.");
                }

                // get template from http server 
                var template = new AgentHttpHelp().GetPrintTemplate_Http();

                //check FilePath whether exist
                var tempFile = new FileInfo(template.FilePath?.Trim());
                if (!tempFile.Exists)
                {
                    throw new Exception("PrintTemplate file not exist.\r\nPath: " + template.FilePath);
                }

                // delete old network printers
                DeleteOldNetPrinters();

                // restore template printers
                return RestoreTemplatePrinters(template.FilePath);
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

        #region + private string RestoreTemplatePrinters(string file)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns>StandardOutput</returns>
        /// <exception cref="StandardError"></exception>
        private string RestoreTemplatePrinters(string file)
        {
            var command = $"{_printBrmExe} -R -NOACL -F {file}";

            var ps = new Process();
            ps.StartInfo.FileName = "cmd.exe";
            ps.StartInfo.UseShellExecute = false;
            ps.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            ps.StartInfo.CreateNoWindow = true;
            ps.StartInfo.RedirectStandardError = true;
            ps.StartInfo.RedirectStandardInput = true;
            ps.StartInfo.RedirectStandardOutput = true;

            var output = new StringBuilder();
            var error = new StringBuilder();

            ps.OutputDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    output.AppendLine(e.Data);
                }
            };

            ps.ErrorDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    error.AppendLine(e.Data);
                }
            };

            ps.Start();
            ps.BeginOutputReadLine();
            ps.BeginErrorReadLine();

            ps.StandardInput.WriteLine(command);
           
            ps.StandardInput.WriteLine("exit");
            ps.WaitForExit((int)new TimeSpan(0,1,0).TotalMilliseconds);

            if (!string.IsNullOrWhiteSpace(error.ToString()))
            {
                throw new Exception("RestoreTemplatePrinters Error: " + error.ToString());
            }

            return output.ToString();
        }
        #endregion
    }
}
