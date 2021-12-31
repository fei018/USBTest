using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using USBNotifyLib;

namespace USBNotifyAgentTray
{
    public partial class App
    {
        private NotifyIcon _trayIcon;
        private ContextMenu _contextMenu;

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
            _contextMenu = new ContextMenu();
            AddTrayIconItem();
            _trayIcon.ContextMenu = _contextMenu;
        }

        private void AddTrayIconItem()
        {
            var updateItem = new MenuItem { Text = "check update" };
            updateItem.Click += UpdateItem_Click;

            _contextMenu.MenuItems.Add(updateItem);
        }

        private void UpdateItem_Click(object sender, EventArgs e)
        {
            try
            {
                Task.Run(() =>
                {
                    _trayPipe.CheckAndUpdateAgent();
                });
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region + private void RemoveTrayIcon()
        private void RemoveTrayIcon()
        {
            if (_contextMenu != null)
            {
                _contextMenu.Dispose();
            }

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
