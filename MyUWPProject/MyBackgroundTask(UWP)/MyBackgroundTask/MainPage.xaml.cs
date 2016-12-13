using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace MyBackgroundTask
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            
        }

       

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //TimeTrigger timetrigger = new TimeTrigger(30, false);

            //var SampleTask = new BackgroundTaskBuilder(); //创建后台任务实例
            //SampleTask.Name = "SimpleBackTask";  //指定后台任务名称
            //SampleTask.TaskEntryPoint = "MyBackgroundTaskSample.MyTask";//指定后台任务名称
            //SampleTask.SetTrigger(timetrigger);//指定后台任务的触发器

            //SystemCondition internetCondition = new SystemCondition(SystemConditionType.InternetAvailable);
            //SampleTask.AddCondition(internetCondition);

            //var access = await BackgroundExecutionManager.RequestAccessAsync();
            //if (access == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity)
            //{
            //    BackgroundTaskRegistration task = SampleTask.Register();
            //    //task.Progress += Value_Progress; ;
            //    //task.Completed += Value_Completed; ;
            //    UpdateUI("", "注册成功");

            //    registerBtn.IsEnabled = false;
            //    UnregisterBtn.IsEnabled = true;

            //    var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            //    settings.Values.Remove(task.Name);
            //}
            //else if (access == BackgroundAccessStatus.Denied)//用户禁用后台任务或后台任务数量已达最大
            //{
            //    await new MessageDialog("您已禁用后台任务或后台任务数量已达最大!").ShowAsync();
            //}

            IBackgroundTrigger m_BKTaskTrigger = new TimeTrigger(15, false);
            IBackgroundTrigger m_BKTaskTrigger2 = new TimeTrigger(18, false);
            

            var triggers = new List<IBackgroundTrigger> { m_BKTaskTrigger, m_BKTaskTrigger2 };
            var conditions = new List<IBackgroundCondition>();

            var requestTask = BackgroundExecutionManager.RequestAccessAsync();

            // NOTE: You have to add System.Linq namespace, and you have to call ToList, otherwise the function will not yield.
            var registrations = RegisterBackgroundTask("MyBackgroundTaskSample.MyTask", "SimpleBackTask", triggers, conditions).ToList();

        }


        public IEnumerable<BackgroundTaskRegistration> RegisterBackgroundTask(String taskEntryPoint, String TaskName,
                                                            IReadOnlyList<IBackgroundTrigger> triggers,
                                                            IReadOnlyList<IBackgroundCondition> conditions)
        {

            

            var builder = new BackgroundTaskBuilder();

            
            builder.TaskEntryPoint = taskEntryPoint;

            BackgroundTaskRegistration registeredTask = null;

            foreach (var condition in conditions)
            {
                if (condition != null)
                {
                    builder.AddCondition(condition);
                }
            }

            //
            // For each trigger we would be registering a different task
            foreach (var trigger in triggers)
            {               
             try
                {
                    if (trigger != null)
                    {
                        builder.SetTrigger(trigger);
                        builder.Name = TaskName + Guid.NewGuid().ToString();
                        registeredTask = builder.Register();
                    }
                    
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: RegisterBackgroundTask - {0}\n\n", ex.Message);
                }

                yield return registeredTask;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == "SimpleBackTask")
                {
                    task.Value.Unregister(true);//删除后台任务
                }
            }
            registerBtn.IsEnabled = true;
            UnregisterBtn.IsEnabled = false;
            UpdateUI("", "后台任务取消");

        }

        void Value_Progress(BackgroundTaskRegistration sender, BackgroundTaskProgressEventArgs args)
        {
            var progress = args.Progress + "%";
            UpdateUI("后台任务进行中:", progress);
        }

        void Value_Completed(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            UpdateUI("100%", "后台任务完成!");
        }

        private async void UpdateUI(string p1, string p2)//更新UI
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                txbprogress.Text = p1;
                txbStatus.Text = p2;
            });
        }
    }
}
