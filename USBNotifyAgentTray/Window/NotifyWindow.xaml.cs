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

            this.Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Bottom - this.Height;
            this.Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Right - this.Width;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtBrand.Text = "";
            TxtProduct.Text = "";

            TxtBrand.Text = TrayPipe.UsbDiskMessage?.Manufacturer;
            TxtProduct.Text = TrayPipe.UsbDiskMessage?.Product;           
        }

        private void BtnRegisterUSB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var reqWin = new RegisterWindow();
                reqWin.Owner = this;

                if (reqWin.ShowDialog().Value)
                {
                    Close();
                }
            }
            catch (Exception)
            {
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
