using MongoDB.Bson;
using MongoDB.Driver;
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

namespace AppMongoDB
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

            var client = new MongoClient("mongodb://10.xxx.xxx.xxx:27017");
            //建立数据库
            var database = client.GetDatabase("uwpmongodbtest");

            var collection = database.GetCollection<BsonDocument>("c1");
            
             var document = new BsonDocument
             {
                                 { "name","MongoDB"},
                 { "type","Database"},
                 { "count",1},
                 { "info",new BsonDocument { { "x", 203 }, { "y", 102 } }}
                             };
                         //插入数据
             await collection.InsertOneAsync(document);
            
             var count = collection.Count(document);
            Debug.WriteLine(count);
            
             //查询数据
             var document1 = collection.Find(document);
            Debug.WriteLine(document1.ToString());
        }
    }
}
