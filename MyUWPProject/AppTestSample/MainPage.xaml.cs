using HtmlCommunicator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AppTestSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.HtmlWebView.Navigate(new Uri("ms-appx-web:///HTMLPage1.html"));
        }

        HtmlCommunicators communicationWinRT = new HtmlCommunicators();
        private void HtmlWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            this.HtmlWebView.AddWebAllowedObject("SCObject", communicationWinRT);
        }
    }
}
