using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using USBNotifyLib;
using USBNotifyAgentTray.USBWindow;

namespace USBNotifyAgentTray
{
    public partial class App
    {
        private NotifyIcon _trayIcon;

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

        #region + private void AddTrayIcon()
        private void AddTrayIcon()
        {
            RemoveTrayIcon();

            _trayIcon = new NotifyIcon
            {
                Icon = USBNotifyAgentTray.Properties.Resources.USB,
                Text = "USB Control",
                Visible = true
            };

            _trayIcon.ContextMenuStrip = new ContextMenuStrip();

            _trayIcon.ContextMenuStrip.Items.Add("Update Setting", null, UpdateSettingItem_Click);
            _trayIcon.ContextMenuStrip.Items.Add("Update Agent", null, UpdateAgentItem_Click);
            _trayIcon.ContextMenuStrip.Items.Add("About", null, AboutItem_Click);
            _trayIcon.ContextMenuStrip.Items.Add("");

            _trayIcon.Visible = true;
        }       
        #endregion

        // Tray Item Click

        #region + private void UpdateAgentSettingItem_Click(object sender, EventArgs e)
        private void UpdateSettingItem_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                try
                {
                    _trayPipe.PushMsg_ToAgent_UpdateSetting();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }
        #endregion

        #region + private void UpdateAgentItem_Click(object sender, EventArgs e)
        private void UpdateAgentItem_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                try
                {
                    _trayPipe.PushMsg_ToAgent_CheckAndUpdateAgent();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Update Agent Error");
                }
            });
        }
        #endregion

        #region + private void AboutItem_Click(object sender, EventArgs e)
        private void AboutItem_Click(object sender, EventArgs e)
        {
            try
            {
                App.Current.Dispatcher.BeginInvoke(new Action(()=>
                {
                    var about = new AboutWin();
                    about.txtAgentVersion.Text = AgentRegistry.AgentVersion;
                    about.Show();
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
    }
}
