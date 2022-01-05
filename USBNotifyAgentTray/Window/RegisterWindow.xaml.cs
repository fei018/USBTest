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
            var email = TxtUserEmail.Text;

            // 檢查 email address format
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    throw new Exception();
                }

                // check email format
                var address = new System.Net.Mail.MailAddress(email);
                if (address.Address != email)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Email format error or required", "Error");
                return;
            }

            // post UsbRegRequest to http server
            try
            {
                if (TrayPipe.UsbDiskMessage == null)
                {
                    throw new Exception("TrayPipe.UsbDiskMessage");
                }

                var post = new UsbRegRequest(TrayPipe.UsbDiskMessage, email.Trim());

                new AgentHttpHelp().PostUsbRegisterRequest(post);

                MessageBox.Show("Send succeed.");
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
