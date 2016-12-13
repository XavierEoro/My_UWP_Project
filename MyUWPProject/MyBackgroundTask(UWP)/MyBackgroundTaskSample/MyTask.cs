using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;

namespace MyBackgroundTaskSample
{
    public sealed class MyTask : IBackgroundTask
    {
        BackgroundTaskDeferral deferral = null;
        IBackgroundTaskInstance bTaskInstance = null;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            bTaskInstance = taskInstance;
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            deferral.Complete();


            
        }

        
    }
}
