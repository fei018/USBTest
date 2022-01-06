using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using USBNotifyLib;

namespace USBNotifyAgentTray
{
    public partial class App
    {
        private NotifyIcon _trayIcon;

        #region + private void AddTrayIcon()
        private void AddTrayIcon()
        {
            RemoveTrayIcon();

            _trayIcon = new NotifyIcon
            {
                Icon = USBNotifyAgentTray.Properties.Resources.USB,
                Text = "USB Notify",
                Visible = true
            };

            _trayIcon.ContextMenuStrip = new ContextMenuStrip();
            _trayIcon.ContextMenuStrip.Items.Add("Update Agent Setting", null, UpdateAgentSettingItem_Click);
            _trayIcon.ContextMenuStrip.Items.Add("Update Usb Filter", null, UpdateUsbFilterDataItem_Click);
            _trayIcon.ContextMenuStrip.Items.Add("Update Agent", null, UpdateAgentItem_Click);
        }

        private void UpdateAgentSettingItem_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                try
                {
                    _trayPipe.UpdateAgentSetting();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        private void UpdateUsbFilterDataItem_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                try
                {
                    _trayPipe.UpdateUsbFilterData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Update Usb Filter Data Error");
                }
            });
        }

        private void UpdateAgentItem_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                try
                {
                    _trayPipe.CheckAndUpdateAgent();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Update Agent Error");
                }
            });
        }
        #endregion

        #region + private void RemoveTrayIcon()
        private void RemoveTrayIcon()
        {
            if (_trayIcon != null)
            {
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
                _trayIcon = null;
            }
        }
        #endregion
    }
}
