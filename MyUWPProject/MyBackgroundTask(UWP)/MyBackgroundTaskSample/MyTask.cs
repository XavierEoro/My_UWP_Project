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
            Button_Click();
        }

        private void Button_Click()
        {
            
            string xml = $@"<toast>
    <visual>
        <binding template='ToastGeneric'>
            <text>Email Audio Toast</text>
            <text>This toast's audio uses one of the system-provided sounds: Notification.Mail</text>
        </binding>
    </visual>

</toast>";
            PushAudioToast(xml);

        }

        private void PushAudioToast(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            var toast = new ToastNotification(doc);
            toast.Dismissed += Toast_Dismissed;
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private void Toast_Dismissed(ToastNotification sender, ToastDismissedEventArgs args)
        {
            Debug.WriteLine("toast dismissed");

            string xml = $@"<toast>
    <visual>
        <binding template='ToastGeneric'>
            <text>toast dismissed</text>
            <text>toast dismissed</text>
        </binding>
    </visual>

</toast>";
            PushAudioToast(xml);

            deferral.Complete();
        }
    }
}
