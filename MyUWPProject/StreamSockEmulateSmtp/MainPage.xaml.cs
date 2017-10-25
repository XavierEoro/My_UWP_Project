using StreamSockEmulateSmtp.Mail;
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

namespace StreamSockEmulateSmtp
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            btnSendMail_Click();
        }

        private async void btnSendMail_Click()
        {
            try
            {
                // Outlook.com
                // HostName : smtp-mail.outlook.com
                // Port : 587
                // SSL : No (upgarde ssl after STARTTLS)

                // Gmail.com
                // HostName : smtp.gmail.com
                // Port : 465
                // SSL : Yes

                // Gmail
                //SmtpClient client = new SmtpClient("smtp.gmail.com", 465, 
                //                                   "your_mail@gmail.com", "password", true);

                
                string smtpServer;
                int port;
                Boolean ssl;
                
                    smtpServer = "smtp-mail.outlook.com";
                    port = 587;
                    ssl = false;
                
                // Outlook
                SmtpClient client = new SmtpClient(smtpServer, port,
                                                    "xiedongwei@hotmail.com", "Xiedw123456@", ssl);

                SmtpMessage message = new SmtpMessage("xiedongwei@hotmail.com",
                                                        "v-doxie@microsoft.com", null, "test", "this is a test message!");

                // adding an other To receiver
                //message.To.Add("Eleanore.Doe@somewhere.com");

                await client.SendMail(message);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }


        }
    }
}
