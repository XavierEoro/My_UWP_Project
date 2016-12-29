using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AppToast
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

        private static async Task<Uri> AudioforToast(string text)
        {
            var voices = SpeechSynthesizer.AllVoices;
            using (var synthesizer = new SpeechSynthesizer())
            {
                if (!String.IsNullOrEmpty(text))
                {
                    try
                    {
                        synthesizer.Voice = voices.First(gender => gender.Gender == VoiceGender.Female);

                        // Create a stream from the text.
                        SpeechSynthesisStream synthesisStream = await synthesizer.SynthesizeTextToStreamAsync(text);

                        // And now write that to a file
                        var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("ToastAudio", CreationCollisionOption.ReplaceExisting);
                        using (var fileStream = await file.OpenStreamForWriteAsync())
                        {
                            await synthesisStream.AsStream().CopyToAsync(fileStream);
                        }

                        // And then return the file path
                        return new Uri("ms-appdata:///temp/ToastAudio");
                    }
                    catch (Exception ex)
                    {
                        // If the text is unable to be synthesized, throw an error message to the user.
                        var messageDialog = new Windows.UI.Popups.MessageDialog(ex.Message);
                        await messageDialog.ShowAsync();
                    }
                }
            }

            // If the text is unable to be synthesized, don't return a custom sound
            return null;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await AudioforToast("hello amy");

            string xml = $@"<toast>
    <visual>
        <binding template='ToastGeneric'>
            <text>Email Audio Toast</text>
            <text>This toast's audio uses one of the system-provided sounds: Notification.Mail</text>
        </binding>
    </visual>

    <audio src='ms-appdata:///temp/ToastAudio'/>

</toast>";
            PushAudioToast(xml);

        }

        private void PushAudioToast(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            var toast = new ToastNotification(doc);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
