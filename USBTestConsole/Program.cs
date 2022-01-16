﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using USBNotifyLib.Win32API;
using USBNotifyLib;
using MailKit.Net.Smtp;
using MimeKit;

namespace USBTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Start...");
                Email();
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("done...");
            Console.ReadLine();
        }

        #region UsbFormProcess
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

                ProcessStartInfo startInfo = new ProcessStartInfo();
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
                AgentLogger.Error(ex.Message);
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
                AgentLogger.Error(ex.Message);
            }
        }

        private void usbProcess_Exited(object sender, EventArgs e)
        {
            if (IsBootUsbForm)
            {
                StartUsbFormProcess();
            }

            AgentLogger.Log("USBFormApp Process Exied Event.");
        }
        #endregion

        #region CreateFile test
        static void CreateFile_OnlyRead()
        {
            //string path = @"\\?\scsi#disk&ven_samsung&prod_mzvlb512hbjq-000#6&25097e69&0&000000#{53f56307-b6bf-11d0-94f2-00a0c91efb8b}";
            string path = @"\\?\Volume{c0fc037c-36db-11ec-838d-8e8f8f399387}";

            var handle = UFileApi.CreateFile_ReadOnly(path);

            Console.WriteLine(handle.ToInt32().ToString("0x"));

            FileTest.Anonymous_ac6e4301_4438_458f_96dd_e86faeeca2a6 f1 = new FileTest.Anonymous_ac6e4301_4438_458f_96dd_e86faeeca2a6
            { Offset = int.MaxValue, OffsetHigh = int.MaxValue >> 32 };

            FileTest.Anonymous_7416d31a_1ce9_4e50_b1e1_0f2ad25c0196 f2 = new FileTest.Anonymous_7416d31a_1ce9_4e50_b1e1_0f2ad25c0196
            {
                Struct1 = f1
            };



            FileTest.OVERLAPPED over = new FileTest.OVERLAPPED() {
                Union1 = f2
            };

            var bl = FileTest.LockFileEx(handle, ref over);

            Console.WriteLine(bl);

            if (!bl)
            {
                Console.WriteLine(new Win32Exception(Marshal.GetLastWin32Error()).Message);
            }

            Console.ReadLine();

            bl = FileTest.UnlockFileEx(handle, ref over);
            
            Console.WriteLine(bl);
            if (!bl)
            {
                Console.WriteLine(new Win32Exception(Marshal.GetLastWin32Error()).Message);
            }

            UFileApi.CloseHandle(handle);
            Console.ReadLine();
        }
        #endregion

        #region email
        static void Email()
        {
            try
            {
                MimeMessage message = new MimeMessage();

                MailboxAddress from = new MailboxAddress("USBAdmin",
                "e_fei_huang@hiphing.com.hk");
                message.From.Add(from);

                MailboxAddress to = new MailboxAddress("fei",
                "e_fei_huang@hiphing.com.hk");
                message.To.Add(to);

                message.Subject = "USBAdmin Test";
                BodyBuilder bodyBuilder = new BodyBuilder { TextBody = "test!!!" };
                message.Body = bodyBuilder.ToMessageBody();

                using (SmtpClient client = new SmtpClient())
                {
                    client.Connect("mailgw.hiphing.com.hk", 25,false);
                    //client.Authenticate("e_fei", "fei");

                    client.Send(message);
                    client.Disconnect(true);
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
