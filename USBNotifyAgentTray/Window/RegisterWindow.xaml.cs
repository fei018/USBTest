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
using USBCommon;

namespace USBNotifyAgentTray
{
    /// <summary>
    /// RequestWindow.xaml 的互動邏輯
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtUserEmail.Text))
            {
                TxtError.Text = "Email is Required";
                return;
            }

            try
            {
                // http register usb
                var post = new PostRegisterUsb
                {
                    UsbInfo = TrayPipe.MessageUsbDisk,
                    UserEmail = TxtUserEmail.Text,
                };

                new UsbHttpHelp().PostRegisterUsb(post);

                MessageBox.Show("Send succeed.");
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                DialogResult = false;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
