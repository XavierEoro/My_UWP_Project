using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.System;
using Windows.System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DiagnosticsSample
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

            DiagnosticAccessStatus diagnosticAccessStatus =
    await AppDiagnosticInfo.RequestAccessAsync();
            switch (diagnosticAccessStatus)
            {
                case DiagnosticAccessStatus.Allowed:
                    Debug.WriteLine("We can get diagnostics for all apps.");
                    GetAllProcessInfo();
                    //DispatcherTimer timer = new DispatcherTimer();
                    //timer.Tick += Timer_Tick;
                    //timer.Interval = TimeSpan.FromSeconds(5);
                    //timer.Start();
                    break;
                case DiagnosticAccessStatus.Limited:
                    Debug.WriteLine("We can only get diagnostics for this app package.");
                    break;
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            GetAllProcessInfo();
        }

        public List<ProcessInfo> ls;
        private async void GetAllProcessInfo()
        {
            ls = new List<ProcessInfo>();
            IReadOnlyList<ProcessDiagnosticInfo> processes = ProcessDiagnosticInfo.GetForProcesses();
            if (processes != null)
            {
                foreach (ProcessDiagnosticInfo process in processes)
                {
                    ProcessInfo pinfo = new ProcessInfo();
                    string exeName = process.ExecutableFileName;
                    pinfo.ExecutableFileName = exeName;
                    string pid = process.ProcessId.ToString();
                    pinfo.ProcessId = pid;
                    ProcessCpuUsageReport cpuReport = process.CpuUsage.GetReport();
                    if (cpuReport != null)
                    {
                        TimeSpan userCpu = cpuReport.UserTime;
                        pinfo.UserTime = userCpu;
                        TimeSpan kernelCpu = cpuReport.KernelTime;
                        pinfo.KernelTime = kernelCpu;
                    }

                    ProcessMemoryUsageReport memReport = process.MemoryUsage.GetReport();
                    if (memReport != null)
                    {
                        ulong npp = memReport.NonPagedPoolSizeInBytes;
                        pinfo.NonPagedPoolSizeInBytes = npp;
                        ulong pp = memReport.PagedPoolSizeInBytes;
                        pinfo.PagedPoolSizeInBytes = pp;
                        ulong peakNpp = memReport.PeakNonPagedPoolSizeInBytes;
                        pinfo.PeakNonPagedPoolSizeInBytes = peakNpp;
                        //...etc
                    }

                    ProcessDiskUsageReport diskReport = process.DiskUsage.GetReport();
                    if (diskReport != null)
                    {
                        long bytesRead = diskReport.BytesReadCount;
                        pinfo.BytesReadCount = bytesRead;
                        long bytesWritten = diskReport.BytesWrittenCount;
                        pinfo.BytesWrittenCount = bytesWritten;
                        //...etc
                    }

                    if (process.IsPackaged)
                    {
                        IList<AppDiagnosticInfo> diagnosticInfos = process.GetAppDiagnosticInfos();
                        if (diagnosticInfos != null && diagnosticInfos.Count > 0)
                        {
                            AppDiagnosticInfo diagnosticInfo = diagnosticInfos.FirstOrDefault();
                            if (diagnosticInfo != null)
                            {
                                IList<AppResourceGroupInfo> groups = diagnosticInfo.GetResourceGroups();
                                if (groups != null && groups.Count > 0)
                                {
                                    AppResourceGroupInfo group = groups.FirstOrDefault();
                                    if (group != null)
                                    {
                                        string name = diagnosticInfo.AppInfo.DisplayInfo.DisplayName;
                                        pinfo.DisplayName = name;
                                        string description = diagnosticInfo.AppInfo.DisplayInfo.Description;
                                        pinfo.Description = description;
                                        BitmapImage bitmapImage = await GetLogoAsync(diagnosticInfo);
                                        pinfo.Logo = bitmapImage;
                                        AppResourceGroupStateReport stateReport = group.GetStateReport();
                                        if (stateReport != null)
                                        {
                                            string executionStatus = stateReport.ExecutionState.ToString();
                                            pinfo.ExecutionState = executionStatus;
                                            string energyStatus = stateReport.EnergyQuotaState.ToString();
                                            pinfo.EnergyQuotaState = executionStatus;
                                        }

                                        AppResourceGroupMemoryReport memoryReport = group.GetMemoryReport();
                                        if (memoryReport != null)
                                        {
                                            AppMemoryUsageLevel level = memoryReport.CommitUsageLevel;
                                            pinfo.CommitUsageLevel = level.ToString();
                                            ulong limit = memoryReport.CommitUsageLimit;
                                            pinfo.CommitUsageLimit = limit;
                                            ulong totalCommit = memoryReport.TotalCommitUsage;
                                            pinfo.TotalCommitUsage = totalCommit;
                                            ulong privateCommit = memoryReport.PrivateCommitUsage;
                                            pinfo.PrivateCommitUsage = privateCommit;
                                            ulong sharedCommit = totalCommit - privateCommit;
                                            pinfo.sharedCommit = sharedCommit;
                                        }
                                    }

                                }
                            }
                        }
                    }
                    ls.Add(pinfo);
                }
            }
            lv.ItemsSource = ls;
        }

        private async Task<BitmapImage> GetLogoAsync(AppDiagnosticInfo app)
        {
            RandomAccessStreamReference stream =
                app.AppInfo.DisplayInfo.GetLogo(new Size(64, 64));
            IRandomAccessStreamWithContentType content = await stream.OpenReadAsync();
            BitmapImage bitmapImage = new BitmapImage();
            await bitmapImage.SetSourceAsync(content);
            return bitmapImage;
        }
    }

    public class ProcessInfo
    {
        public string ExecutableFileName { get; set; }
        public string ProcessId { get; set; }
        public TimeSpan UserTime { get; set; }
        public TimeSpan KernelTime { get; set; }

        public ulong NonPagedPoolSizeInBytes { get; set; }

        public ulong PagedPoolSizeInBytes { get; set; }

        public ulong PeakNonPagedPoolSizeInBytes { get; set; }
        public long BytesReadCount { get; set; }

        public long BytesWrittenCount { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public BitmapImage Logo { get; set; }

        public string ExecutionState { get; set; }

        public string EnergyQuotaState { get; set; }

        public string CommitUsageLevel { get; set; }

        public ulong CommitUsageLimit { get; set; }
        public ulong TotalCommitUsage { get; set; }
        public ulong PrivateCommitUsage { get; set; }
        public ulong sharedCommit { get; set; }
    }
}
