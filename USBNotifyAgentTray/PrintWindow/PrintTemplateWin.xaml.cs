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
using USBNotifyAgentTray;

namespace USBNotifyAgentTray.PrintWindow
{
    /// <summary>
    /// PrintTemplateWin.xaml 的互動邏輯
    /// </summary>
    public partial class PrintTemplateWin : Window
    {
        public PrintTemplateWin()
        {
            InitializeComponent();

            App.TrayPipe.AddPrintTemplateCompletedEvent += TrayPipe_AddPrintTemplateCompletedEvent;
        }

        private void TrayPipe_AddPrintTemplateCompletedEvent(object sender, USBNotifyLib.PipeEventArgs e)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                btnAddPrinter.IsEnabled = true;
                btnCancel.IsEnabled = true;

                progressBar1.Visibility = Visibility.Hidden;
                txtResult.Text = e.Msg;
            }));
        }

        private void btnAddPrinter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnAddPrinter.IsEnabled = false;
                btnCancel.IsEnabled = false;

                progressBar1.Visibility = Visibility.Visible;
                App.TrayPipe?.PushMsg_ToAgent_AddPrintTemplate();
            }
            catch (Exception)
            {
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
