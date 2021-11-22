using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USBNotifyAgentDesktop
{
    public partial class USBNotifyAgentDesktopForm : Form
    {
        public USBNotifyAgentDesktopForm()
        {
            InitializeComponent();
        }

        private void USBNotifyAgentDesktopForm_Shown(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void NotifyItem_About_Click(object sender, EventArgs e)
        {
            new AboutForm().Show();
        }
    }
}
