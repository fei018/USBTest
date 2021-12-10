using System;
using System.Windows.Forms;

namespace USBNotifyAgentTray
{
    public partial class App
    {
        private NotifyIcon _trayIcon;
        private ContextMenu _contextMenu;

        #region TrayIcon
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
            var aboutItem = new MenuItem { Text = "About" };
            aboutItem.Click += (s, e) => { };

            _contextMenu.MenuItems.Add(aboutItem);
        }

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
