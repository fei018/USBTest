using System;
using System.IO;
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
            UsbStart();
        }

        #region UsbStart()
        private void UsbStart()
        {
            // 從 registry 加載 config
            UsbConfig.ReloadRegistryConfig();

            if (UsbConfig.UsbFilterEnabled)
            {
                new UsbFilterDb().Reload_UsbFilterDb();
                new UsbFilter().Filter_Scan_All_USB_Disk();
                UsbHttpHelp.Set_GetUsbFilterDb_Http_Timer();
            }           
        }
        #endregion

        #region OnUsbInterface(UsbEventDeviceInterfaceArgs args)
        /// <summary>
        /// Post Usb plugin history to server
        /// </summary>
        /// <param name="args"></param>
        public override void OnUsbInterface(UsbEventDeviceInterfaceArgs args)
        {
            if (args.Action == UsbDeviceChangeEvent.Arrival)
            {
                if (args.DeviceInterface == UsbMonitor.UsbDeviceInterface.Disk)
                {
                    Task.Run(() =>
                    {
                        new UsbHttpHelp().PostComputerUsbHistoryInfo_byDisk_Http(args.Name);
                    });
                }
            }
        }
        #endregion

        #region OnUsbVolume(UsbEventVolumeArgs args)
        private readonly object _Locker_OnVolume = new object();
        public override void OnUsbVolume(UsbEventVolumeArgs args)
        {
            if (UsbConfig.UsbFilterEnabled)
            {
                lock (_Locker_OnVolume)
                {
                    if (args.Flags == UsbVolumeFlags.Net) return;

                    try
                    {
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
                    catch (Exception) { }
                }
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
