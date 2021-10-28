using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsbMonitor;
using System.Windows.Forms;
using USBNetLib;

namespace USBNetLib.Notify
{
    public class NotifyUSBForm : UsbMonitorForm
    {
        /// <summary>
        /// send window handle
        /// </summary>
        public event EventHandler<IntPtr> ShownNotifyUSBForm;

        public NotifyUSBForm()
        {
            this.Shown += NotifyUSBForm_Shown;
        }

        /// <summary>
        /// form shown event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyUSBForm_Shown(object sender, EventArgs e)
        {
            ShownNotifyUSBForm?.Invoke(this, this.Handle);
        }

        public override void OnUsbVolume(UsbEventVolumeArgs args)
        {
            Task.Run(() =>
            {
                if (args.Flags == UsbVolumeFlags.Net)
                {
                    return;
                }

                try
                {
                    if (args.Action == UsbDeviceChangeEvent.Arrival)
                    {
                        new UsbRuleFilter().Filter_NotifyUSB_Use_DriveLetter(args.Drives[0]);
                    }
                }
                catch (Exception)
                {
                }              
            });
        }
    }
}
