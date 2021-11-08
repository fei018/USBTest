using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UsbMonitor;
using USBNotifyLib;

namespace USBNotifyApp
{
    public partial class USBNofityForm : UsbMonitorForm
    {
        public USBNofityForm()
        {
            InitializeComponent();
            UsbStart();
        }

        #region + private void UsbStart()
        private void UsbStart()
        {
            new UsbFilterTable().Reload_PolicyUSBTable();
            new UsbFilter().Filter_Scan_All_USB_Disk();

            UsbManager.Set_UpdateUsbFilterTable_Http_Timer();
        }
        #endregion

        #region + public override void OnUsbVolume(UsbEventVolumeArgs args)
        private readonly object _Locker_OnVolume = new object();
        public override void OnUsbVolume(UsbEventVolumeArgs args)
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
                                new UsbFilter().Filter_NotifyUSB_Use_DriveLetter(letter);
                            });
                        }
                    }
                }
                catch (Exception) { }
            }
        }
        #endregion

        #region + private void USBNofityForm_FormClosed(object sender, FormClosedEventArgs e)
        private void USBNofityForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            base.Stop();
        }
        #endregion

        #region + private void USBNofityForm_Shown(object sender, EventArgs e)
        private void USBNofityForm_Shown(object sender, EventArgs e)
        {
            this.Hide();
        }
        #endregion

        
    }
}
