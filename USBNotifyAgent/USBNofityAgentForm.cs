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
        }

        #region OnUsbInterface(UsbEventDeviceInterfaceArgs args)
        /// <summary>
        /// Post Usb plugin history to server
        /// </summary>
        /// <param name="args"></param>
        public override void OnUsbInterface(UsbEventDeviceInterfaceArgs args)
        {
            try
            {
                if (AgentManager.IsUsbHistoryEnable)
                {
                    if (args.Action == UsbDeviceChangeEvent.Arrival)
                    {
                        if (args.DeviceInterface == UsbMonitor.UsbDeviceInterface.Disk)
                        {
                            Task.Run(() =>
                            {
                                new UsbHttpHelp().PostPerUsbHistory_byDisk_Http(args.Name);
                            });
                        }
                    }
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
                if (AgentManager.IsUsbFilterEnable)
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
                                    new UsbFilter().Filter_NotifyUsb_Use_DriveLetter(letter);
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

    }
}
