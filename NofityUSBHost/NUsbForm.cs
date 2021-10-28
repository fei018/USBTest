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

namespace NofityUSBHost
{
    public partial class NUsbForm : UsbMonitorForm
    {
        public NUsbForm()
        {
            InitializeComponent();
        }

        public override void OnUsbVolume(UsbEventVolumeArgs args)
        {
            
        }
    }
}
