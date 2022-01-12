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
using System.Windows.Navigation;
using System.Windows.Shapes;
using USBNotifyLib;

namespace USBNotifyAgentTray
{
    /// <summary>
    /// RequestPage.xaml 的互動邏輯
    /// </summary>
    public partial class RequestPage : Page
    {
        public RequestPage()
        {
            InitializeComponent();
        }

        public event EventHandler<string> USBRequestSubmittedEvent;

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {           
            var select = ComboBoxEmail.SelectedItem as ComboBoxItem;
            var email = TxtUserEmail.Text?.Trim() + select?.Content;

            // 檢查 email address format
            try
            {
                // check email format
                var address = new System.Net.Mail.MailAddress(email);
                if (address.Address != email)
                {
                    throw new Exception("Email format error.");
                }

                if (string.IsNullOrWhiteSpace(TxtReason.Text))
                {
                    throw new Exception("Request reason is empty.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                return;
            }

            // post UsbRegRequest to http server
            try
            {
                if (TrayPipe.UsbDiskInfo == null)
                {
                    throw new Exception("TrayPipe.UsbDiskInfo");
                }

                var post = new UsbRequest(TrayPipe.UsbDiskInfo, email, TxtReason.Text?.Trim());

                new AgentHttpHelp().PostUsbRegisterRequest(post);

                USBRequestSubmittedEvent?.Invoke(this, "Your request was submitted successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
