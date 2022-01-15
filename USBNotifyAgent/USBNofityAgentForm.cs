﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using UsbMonitor;
using USBNotifyLib;

namespace USBNotifyAgent
{
    public partial class USBNofityAgentForm : UsbMonitorForm
    {
        public USBNofityAgentForm()
        {
            InitializeComponent();

            AgentPipeStart();

            AgentManager.Startup();
        }

        // AgentPipe
        #region AgentPipe

        private PipeServerAgent _agentPipe;

        private void AgentPipeStart()
        {
            _agentPipe = new PipeServerAgent();
            _agentPipe.CloseAgentFormEvent += (s, e) => { this.Close(); };
            _agentPipe.Start();
        }
        #endregion

        // form
        #region USBNofityAgentForm_FormClosed(object sender, FormClosedEventArgs e)
        private void USBNofityAgentForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            base.Stop();
            _agentPipe.Stop();
        }
        #endregion

        // OnUsbInterface

        #region + public override void OnUsbInterface(UsbEventDeviceInterfaceArgs args)
        public override void OnUsbInterface(UsbEventDeviceInterfaceArgs args)
        {
            if (args.Action == UsbDeviceChangeEvent.Arrival)
            {
                if (args.DeviceInterface == UsbMonitor.UsbDeviceInterface.Disk)
                {
                    FilterUsbDisk(args.Name);

                    CheckUsbWhitelist_PluginUSB(args.Name);

                    PostUsbHistoryToHttpServer(args.Name);
                }
            }
        }
        #endregion

        #region + private void PostUsbHistoryToHttpServer()
        private void PostUsbHistoryToHttpServer(string diskPath)
        {
            Task.Run(() =>
            {
                try
                {
                    if (!AgentRegistry.UsbHistoryEnabled) return;

                    // post usb history to server
                    new AgentHttpHelp().PostPerUsbHistory_byDisk_Http(diskPath);
                }
                catch (Exception)
                {
                }
            });
        }
        #endregion

        #region + private void CheckUsbWhitelist_PluginUSB()
        private void CheckUsbWhitelist_PluginUSB(string diskPath)
        {
            Task.Run(() =>
            {
                try
                {
                    if (!AgentRegistry.UsbFilterEnabled) return;

                    // push usbmessage to agent tray pipe
                    var usb = new UsbFilter().Find_UsbDisk_By_DiskPath(diskPath);
                    if (!UsbWhitelistHelp.IsFind(usb))
                    {
                        _agentPipe.PushMsg_ToTray_UsbDiskNotInWhitelist(usb);
                    }
                }
                catch (Exception)
                {
                }
            });
        }
        #endregion

        #region + private void FilterUsbDisk(string diskPath)
        private void FilterUsbDisk(string diskPath)
        {
            Task.Run(() =>
            {
                try
                {
                    if (!AgentRegistry.UsbFilterEnabled) return;

                    var disk = new UsbDisk { DiskPath = diskPath };
                    new UsbFilter().Filter_UsbDisk_By_DiskPath(disk);
                }
                catch (Exception)
                {
                }
            });
        }
        #endregion

        // OnUsbVolume

        #region + public override void OnUsbVolume(UsbEventVolumeArgs args)
        //public override void OnUsbVolume(UsbEventVolumeArgs args)
        //{
        //    if (args.Action == UsbDeviceChangeEvent.Arrival)
        //    {
        //        if (args.Flags == UsbVolumeFlags.Media)
        //        {
        //            foreach (char letter in args.Drives)
        //            {
        //                FilterUsbDisk(letter);
        //            }
        //        }
        //    }
        //}
        #endregion

        #region + private void FilterUsbDisk(char letter)
        //private void FilterUsbDisk(char letter)
        //{
        //    Task.Run(() =>
        //    {
        //        try
        //        {
        //            if (!AgentRegistry.UsbFilterEnabled) return;

        //            new UsbFilter().Filter_UsbDisk_By_DriveLetter(letter);
        //        }
        //        catch (Exception)
        //        {
        //        }
        //    });
        //}
        #endregion

    }
}
