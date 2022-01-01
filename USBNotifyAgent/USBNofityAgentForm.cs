using System;
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

            AgentManager.Startup();

            _agentPipe.Start();
        }

        private AgentPipe _agentPipe = new AgentPipe();

        // form
        #region USBNofityForm_FormClosed(object sender, FormClosedEventArgs e)
        private void USBNofityForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            base.Stop();
        }
        #endregion

        #region USBNofityForm_Shown(object sender, EventArgs e)
        private void USBNofityForm_Shown(object sender, EventArgs e)
        {
            // this.Hide();
        }
        #endregion

        //

        #region OnUsbInterface(UsbEventDeviceInterfaceArgs args)
        public override void OnUsbInterface(UsbEventDeviceInterfaceArgs args)
        {
            try
            {
                if (AgentRegistry.UsbHistoryEnabled)
                {
                    if (args.Action != UsbDeviceChangeEvent.Arrival)
                    {
                        return;
                    }
                    if (args.DeviceInterface != UsbMonitor.UsbDeviceInterface.Disk)
                    {
                        return;
                    }
                    Task.Run(() =>
                    {
                        // push usbmessage to agent tray pipe
                        var usb = new UsbFilter().Find_UsbDisk_Use_DiskPath(args.Name);
                        _agentPipe.PushUsbMessage(usb);

                        // post usb history to server
                        //new UsbHttpHelp().PostPerUsbHistory_byDisk_Http(args.Name);
                    });
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region OnUsbVolume(UsbEventVolumeArgs args)
        private readonly object _Locker_OnVolume = new object();
        public override void OnUsbVolume(UsbEventVolumeArgs args)
        {
            try
            {
                if (AgentRegistry.UsbFilterEnabled)
                {
                    lock (_Locker_OnVolume)
                    {
                        if (args.Flags == UsbVolumeFlags.Net) return;

                        if (args.Action == UsbDeviceChangeEvent.Arrival)
                        {
                            foreach (char letter in args.Drives)
                            {
                                Task.Run(() =>
                                {
                                    new UsbFilter().Filter_UsbDisk_Use_DriveLetter(letter);
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion
    }
}
