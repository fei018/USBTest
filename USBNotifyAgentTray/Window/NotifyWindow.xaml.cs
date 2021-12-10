using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using USBNotifyLib;

namespace USBNotifyAgentTray
{
    /// <summary>
    /// NotifyWindow.xaml 的互動邏輯
    /// </summary>
    public partial class NotifyWindow : Window
    {
        public NotifyWindow()
        {
            InitializeComponent();
        }

        public NotifyUsb NotifyUsb { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtUsbDetail.Text = NotifyUsb?.ToString();
        }
    }
}
