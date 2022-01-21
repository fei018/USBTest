using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using USBNotifyLib;
using USBNotifyAgentTray.USBWindow;
using System.Diagnostics;
using System.IO;
using USBNotifyAgentTray.PrintWindow;

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
                Icon = USBNotifyAgentTray.Properties.Resources.icon,
                Text = "IT Support Tools",
                Visible = true
            };

            _trayIcon.ContextMenuStrip = new ContextMenuStrip();

            _trayIcon.ContextMenuStrip.Items.Add("Update Setting", null, UpdateSettingItem_Click);
            _trayIcon.ContextMenuStrip.Items.Add("Update Agent", null, UpdateAgentItem_Click);
            _trayIcon.ContextMenuStrip.Items.Add("Remote Support", null, RunRemoteSupportVNC_Click);
            _trayIcon.ContextMenuStrip.Items.Add("Set Printer", null, SetPrinter_Click);
            _trayIcon.ContextMenuStrip.Items.Add("About", null, AboutItem_Click);
            _trayIcon.ContextMenuStrip.Items.Add("");

            _trayIcon.Visible = true;
        }
        #endregion

        // Tray Item Click

        #region UpdateSettingItem_Click
        private void UpdateSettingItem_Click(object sender, EventArgs e)
        {
            try
            {
                TrayPipe.PushMsg_ToAgent_UpdateSetting();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region UpdateAgentItem_Click
        private void UpdateAgentItem_Click(object sender, EventArgs e)
        {
            try
            {
                TrayPipe.PushMsg_ToAgent_CheckAndUpdateAgent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Update Agent Error");
            }
        }
        #endregion

        #region RunRemoteSupportVNC_Click
        private void RunRemoteSupportVNC_Click(object sender, EventArgs e)
        {
            try
            {
                var vnc = Path.Combine(AgentRegistry.RemoteSupportExe);
                Process.Start(vnc);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region SetPrinter_Click
        private void SetPrinter_Click(object sender, EventArgs e)
        {
            try
            {
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        var prnWin = new PrintTemplateWin();
                        prnWin.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Set Printer");
                    }
                }));
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region AboutItem_Click
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
