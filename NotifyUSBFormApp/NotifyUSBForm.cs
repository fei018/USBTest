using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UsbMonitor;
using USBNetLib;

namespace NotifyUSBFormApp
{
    public partial class NotifyUSBForm : UsbMonitorForm
    {
        private readonly object _Locker_OnVolume = new object();

        public NotifyUSBForm()
        {
            InitializeComponent();

            UsbStart();
        }

        #region hide form
        private void NotifyUSBForm_Shown(object sender, EventArgs e)
        {
            //this.Hide();
        }
        #endregion

        #region disable USB Notificaition when form closed
        private void NotifyUSBForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // disable USB Notificaition
            base.Stop();
        }
        #endregion

        #region + private void UsbStart()
        private void UsbStart()
        {
            new PolicyUSBTable().Reload_PolicyUSBTable();
            new UsbPolicyFilter().Filter_Scan_All_USB_Disk();

            UsbPolicyUpdate.Set_HttpGetPolicy_Timer();
        }
        #endregion

        #region + public override void OnUsbVolume(UsbEventVolumeArgs args)
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
                                new UsbPolicyFilter().Filter_NotifyUSB_Use_DriveLetter(letter);
                            });
                        }
                    }
                }
                catch (Exception) { }
            }
        }
        #endregion

    }
}
