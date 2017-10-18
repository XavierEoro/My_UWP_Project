using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Windows.Web.Http;
using Windows.Web.Http.Filters;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SimulateWebLogin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        HttpClient client;
        private static Uri qiushiaddress = new Uri("https://www.cnblog.com");
        private static Uri qiushilogin = new Uri("https://www.qiushibaike.com/new4/session");
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            DeleteCookie();
            client = new HttpClient();
            await client.GetAsync(qiushiaddress);
            base.OnNavigatedTo(e);
            try
            {

                HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                HttpCookieCollection cookieCollection = filter.CookieManager.GetCookies(qiushiaddress);
                string _xsrf = string.Empty;
                foreach (HttpCookie cookie in cookieCollection)
                {
                    Debug.WriteLine(cookie.Name);
                    Debug.WriteLine(cookie.Value);
                    if (cookie.Name == "_xsrf")
                    {
                        _xsrf = cookie.Value;
                        client.DefaultRequestHeaders.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue(cookie.Name, cookie.Value));
                    }
                }
                client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.81 Safari/537.36");
                client.DefaultRequestHeaders.Accept.Add(new Windows.Web.Http.Headers.HttpMediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                //client.DefaultRequestHeaders.TryAppendWithoutValidation("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                client.DefaultRequestHeaders.Add("Referer", "https://www.qiushibaike.com/");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.8");

                HttpMultipartFormDataContent form = new HttpMultipartFormDataContent();
                form.Add(new HttpStringContent(""), "login");
                form.Add(new HttpStringContent(""), "password");
                form.Add(new HttpStringContent("checked"), "remember_me");
                form.Add(new HttpStringContent("1298"), "duration");
                form.Add(new HttpStringContent(_xsrf), "_xsrf");
                HttpResponseMessage response = await client.PostAsync(qiushilogin, form);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void DeleteCookie()
        {
            try
            {
                HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                HttpCookieCollection cookieCollection = filter.CookieManager.GetCookies(qiushiaddress);
                foreach (HttpCookie cookie in cookieCollection)
                {
                    filter.CookieManager.DeleteCookie(cookie);
                }
            }
            catch (ArgumentException ex)
            {

            }

        }
    }
}
