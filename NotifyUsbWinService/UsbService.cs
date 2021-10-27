using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using USBNetLib;

namespace NotifyUsbWinService
{
    public partial class UsbService : ServiceBase
    {
        public UsbService()
        {
            InitializeComponent();
        }

        private USBManager _usbManager;

        protected override void OnStart(string[] args)
        {
            _usbManager?.Close();

            _usbManager = new USBManager();

            _usbManager.Start();
        }

        protected override void OnStop()
        {
            _usbManager?.Close();
        }
    }
}
