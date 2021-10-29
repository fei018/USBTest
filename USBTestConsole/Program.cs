﻿using NotifyUSBFormApp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBNetLib;

namespace USBTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Start...");

                var p = new Program();

                p.OnStart();

                Console.ReadLine();

                p.OnStop();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }


        private Process _usbFormProcess;
        private bool IsBootUsbForm = true;

        void OnStart()
        {
            IsBootUsbForm = true;
            StartUsbFormProcess();
        }

        void OnStop()
        {
            IsBootUsbForm = false;
            CloseUsbFormProcess();
        }

        private void StartUsbFormProcess()
        {
            try
            {
                CloseUsbFormProcess();

                ProcessStartInfo startInfo = new ProcessStartInfo(NotifyUSBForm.NotifyUSBAppPath);
                _usbFormProcess = new Process
                {
                    EnableRaisingEvents = true,
                    StartInfo = startInfo
                };

                _usbFormProcess.Exited += usbProcess_Exited;

                _usbFormProcess.Start();
            }
            catch (Exception ex)
            {
                USBLogger.Error(ex.Message);
            }
        }

        private void CloseUsbFormProcess()
        {
            try
            {
                if (_usbFormProcess != null && !_usbFormProcess.HasExited)
                {
                    if (!_usbFormProcess.CloseMainWindow())
                    {
                        _usbFormProcess.Kill();
                    }
                    _usbFormProcess.Close();
                }
            }
            catch (Exception ex)
            {
                USBLogger.Error(ex.Message);
            }
        }

        private void usbProcess_Exited(object sender, EventArgs e)
        {
            if (IsBootUsbForm)
            {
                StartUsbFormProcess();
            }

            USBLogger.Log("USBFormApp Process Exied Event.");
        }
    }
}
