using Microsoft.WindowsAzure.MobileServices;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AppZaure
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

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var mobileClient = new MobileServiceClient("https://bruce-mobile.azurewebsites.net");
            var table = mobileClient.GetTable<phoneuser>();
            var result = await table.Where(t=>t.name != null).ToCollectionAsync();
            foreach (var r in result)
            {
                Debug.WriteLine(r.id+" "+r.createdAt+" "+r.updatedAt+" "+r.version+" "+r.deleted+" "+r.name+" "+r.weight+" "+r.height+" "+r.age+" "+r.gender+" "+r.password);
            }


            var _result = result.GroupBy(r=>r.name).Select(p=>p.FirstOrDefault()).ToList();
            
            foreach (var r in _result)
            {
                //Debug.WriteLine(r.id + " " + r.createdAt + " " + r.updatedAt + " " + r.version + " " + r.deleted + " " + r.name + " " + r.weight + " " + r.height + " " + r.age + " " + r.gender + " " + r.password);
            }
        }
    }

    public class phoneuser
    {
        public string id { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string version { get; set; }
        public bool deleted { get; set; }

        public string name { get; set; }
        public double weight { get; set; }

        public double height { get; set; }
        public int age { get; set; }
        public bool gender { get; set;}
        public string password { get; set; }
    }
}
