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

            ShowRequestPage();
        }

        private void ShowRequestPage()
        {
            var requestPage = new RequestPage();
            requestPage.RequestPage_SubmittedEvent += RequestPage_USBRequestSubmittedEvent;

            ContentCtrl_ReuqestPage.Content = new Frame { Content = requestPage };
            
        }

        /// <summary>
        /// 按下 Submit 按鈕 事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RequestPage_USBRequestSubmittedEvent(object sender, string e)
        {
            var result = new RequestResultPage();
            result.TxtResult.Text = e;

            ContentCtrl_ReuqestPage.Content = new Frame { Content = result };
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
