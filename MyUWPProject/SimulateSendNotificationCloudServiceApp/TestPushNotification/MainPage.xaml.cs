using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.PushNotifications;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestPushNotification
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string SAMPLE_TASK_NAME = "ReceiveRawBackgroundTask";
        private const string SAMPLE_TASK_ENTRY_POINT = "RawBackgroundTask.ReceiveRawBackgroundTask";
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            Debug.WriteLine(channel.Uri);
            txtchannelURL.Text = channel.Uri;
        }

        private async void Button_Click_OutofProcess(object sender, RoutedEventArgs e)
        {
            // Applications must have lock screen privileges in order to receive raw notifications
            BackgroundAccessStatus backgroundStatus = await BackgroundExecutionManager.RequestAccessAsync();

            // Make sure the user allowed privileges
            if (backgroundStatus != BackgroundAccessStatus.Denied && backgroundStatus != BackgroundAccessStatus.Unspecified)
            {
                RegisterTask();
            }
            else
            {
                Debug.WriteLine("Cannot register background task.");
            }
        }

        private void RegisterTask()
        {
            // Open the channel. See the "Push and Polling Notifications" sample for more detail
            try
            {
                UnregisterBackgroundTask();
                RegisterBackgroundTask();
                Debug.WriteLine("Task registered");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not create a channel. Error number:" + ex.Message);
            }
        }

        private void RegisterBackgroundTask()
        {
            BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
            PushNotificationTrigger trigger = new PushNotificationTrigger();
            taskBuilder.SetTrigger(trigger);

            // Background tasks must live in separate DLL, and be included in the package manifest
            // Also, make sure that your main application project includes a reference to this DLL
            taskBuilder.TaskEntryPoint = SAMPLE_TASK_ENTRY_POINT;
            taskBuilder.Name = SAMPLE_TASK_NAME;

            try
            {
                BackgroundTaskRegistration task = taskBuilder.Register();
                task.Completed += BackgroundTaskCompleted;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Registration error: " + ex.Message);
                UnregisterBackgroundTask();
            }
        }

        private bool UnregisterBackgroundTask()
        {
            foreach (var iter in BackgroundTaskRegistration.AllTasks)
            {
                IBackgroundTaskRegistration task = iter.Value;
                if (task.Name == SAMPLE_TASK_NAME)
                {
                    task.Unregister(true);
                    return true;
                }
            }
            return false;
        }

        private void BackgroundTaskCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
             Debug.WriteLine("Background work item triggered by raw notification with payload = " + ApplicationData.Current.LocalSettings.Values[SAMPLE_TASK_NAME] + " has completed!");
        }

        private async void Button_Click_InProcessTask(object sender, RoutedEventArgs e)
        {
            BackgroundAccessStatus backgroundStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundStatus != BackgroundAccessStatus.DeniedByUser && backgroundStatus != BackgroundAccessStatus.Unspecified)
            {
                RegisterInProcessBackgroundTask();
            }
            else
            {
                Debug.WriteLine("Cannot register background task.");
            }
        }

        private void RegisterInProcessBackgroundTask()
        {
            BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
            PushNotificationTrigger trigger = new PushNotificationTrigger();
            taskBuilder.SetTrigger(trigger);

            // Background tasks must live in separate DLL, and be included in the package manifest
            // Also, make sure that your main application project includes a reference to this DLL
            //taskBuilder.TaskEntryPoint = SAMPLE_TASK_ENTRY_POINT;
            taskBuilder.Name = SAMPLE_TASK_NAME;

            try
            {
                BackgroundTaskRegistration task = taskBuilder.Register();
                task.Completed += BackgroundTaskCompleted;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Registration error: " + ex.Message);
                UnregisterBackgroundTask();
            }
        }
    }
}
