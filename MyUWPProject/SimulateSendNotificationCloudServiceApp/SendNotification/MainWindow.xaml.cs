using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace SendNotification
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            txtpushxmlstring.Text = toast;
        }
        public Dictionary<FlowSteps, string> steps;
        public string toast = string.Format(@"<toast><visual><binding template=""ToastText01""><text id=""1"">{0}</text></binding></visual></toast>", "hello world!!!");
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SendNitification WnsNotify = new SendNitification();
            ComboBoxItem item = pushtype.SelectedItem as ComboBoxItem;
            steps = WnsNotify.SendNotifytoWNS(content: txtpushxmlstring.Text, NotififyType: item.Content.ToString(), url: txtChannelRUL.Text);




            var kvs = steps.Select(kvp => string.Format("\"{0}\":\"{1}\"", kvp.Key, string.Join(",", kvp.Value.Replace("\"", "\\\""))));

            string jsonResult = string.Concat("{", string.Join(",", kvs), "}");
            jsonResult = jsonResult.Replace('"', '\"');
            Debug.WriteLine(jsonResult);
            txtresponseCode.Text = jsonResult;
        }
    }
}
