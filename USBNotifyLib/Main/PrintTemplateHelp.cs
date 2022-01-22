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

        private static string _localTemplateFile;


        #region + public void Start()
        /// <summary>
        /// 
        /// </summary>
        /// <returns>StandardOutput</returns>
        /// <exception cref="StandardError"></exception>
        public static string Start()
        {
            try
            {
                // check PrintBrm.exe
                if (!File.Exists(_printBrmExe))
                {
                    throw new Exception("PrintBrm.exe not exist.");
                }

                // restore template printers
                var output = RestoreTemplatePrinters(_localTemplateFile);

                // delete old network printers
                DeleteOldNetPrinters();

                return output;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + private void DeleteOldNetPrinters()
        private static void DeleteOldNetPrinters()
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
        private static string RestoreTemplatePrinters(string file)
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

        #region + public static void CopyTemplateFileToLocal(FileInfo sourceFileInfo)
        public static void CopyTemplateFileToLocal(FileInfo sourceFileInfo)
        {
            try
            {
                _localTemplateFile = Path.Combine(AgentRegistry.AgentDataDir, sourceFileInfo.Name);
                sourceFileInfo.CopyTo(_localTemplateFile, true);

                if (!File.Exists(_localTemplateFile))
                {
                    throw new Exception("Print template file copy fail.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
