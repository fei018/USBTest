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
    public partial class RequestWindow : Window
    {
        public RequestWindow()
        {
            InitializeComponent();

            ShowPage();
        }

        private void ShowPage()
        {
            var requestPage = new RequestPage();
            requestPage.USBRequestSubmittedEvent += RequestPage_USBRequestSubmittedEvent;

            CtlReuqestPage.Content = new Frame { Content = requestPage };
            
        }

        private void RequestPage_USBRequestSubmittedEvent(object sender, string e)
        {
            var result = new RequestResultPage();
            result.TxtResult.Text = e;

            CtlReuqestPage.Content = new Frame { Content = result };
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
