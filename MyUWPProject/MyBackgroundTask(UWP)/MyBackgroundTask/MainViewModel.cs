using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MyBackgroundTask
{
    class MainViewModel
    {
        public static object Dispatcher { get; private set; }

        public static void Test()
        {
            var webView = new WebView(WebViewExecutionMode.SeparateThread);
            webView.Navigate(new Uri("http://www.msdn.com"));
            webView.LoadCompleted += WebView_LoadCompleted;

            //Task.Factory.StartNew(() =>
            //{
            //    var tmp = new WebView(WebViewExecutionMode.SeparateThread); // Exception here
            //    tmp.Navigate(new Uri("http://www.msdn.com"));
            //    tmp.LoadCompleted += WebView_LoadCompleted;
            //});

        }

        private static void WebView_LoadCompleted(object sender, NavigationEventArgs e)
        {
            
        }
    }
}
