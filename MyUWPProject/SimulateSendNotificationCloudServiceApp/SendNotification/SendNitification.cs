using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SendNotification
{
    public class OAuthToken
    {
        [DataMember(Name = "access_token")]
        public string access_token { get; set; }
        [DataMember(Name = "expires_in")]
        public string expires_in { get; set; }

        [DataMember(Name = "token_type")]
        public string token_type { get; set; }
    }

    public class SendNitification
    {
        public const string clientSecret = "McXJ1RvrmJOMM1wg8JYtMDW";
        public const string grant_type = "client_credentials";
        public const string packageSID = "ms-app://s-1-15-2-233892466-3397685525-3203054955-3266348075-3405961389-908128529-465990943";
        public const string scope = "notify.windows.com";
        public void GetTokenFromWNS(out Dictionary<FlowSteps, string> steps)
        {
            steps = new Dictionary<FlowSteps, string>();
            string url = "https://login.live.com/accesstoken.srf";
            steps[FlowSteps.TokenUrl] = url;
            string data = string.Format("grant_type={0}&client_id={1}&client_secret={2}&scope={3}",
                grant_type,
               HttpUtility.UrlEncode(packageSID),
               clientSecret,
               scope);
            steps[FlowSteps.SID] = packageSID;
            steps[FlowSteps.ClientSecret] = clientSecret;
            //steps.Add("");
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            request.ContentLength = bytes.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            string result = "";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream);
                    result = reader.ReadToEnd();

                }
            }

            steps[FlowSteps.Token] = GetOAuthTokenFromJson(result).access_token;
            //return GetOAuthTokenFromJson(result).AccessToken;
            // steps["token"]= GetOAuthTokenFromJson(result).AccessToken;

        }
        public Dictionary<FlowSteps, string> SendNotifytoWNS(string content, string NotififyType, string url)
        {
            Dictionary<FlowSteps, string> steps = new Dictionary<FlowSteps, string>();

            try
            {
                var accessToken = string.Empty;
                GetTokenFromWNS(out steps);
                var toast = string.Format(@"<toast><visual><binding template=""ToastText01""><text id=""1"">{0}</text></binding></visual></toast>", "hello world!!!");
                steps[FlowSteps.RequestContent] = content;
                accessToken = steps[FlowSteps.Token];

                byte[] contentInBytes = Encoding.UTF8.GetBytes(content);


                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                steps[FlowSteps.RequestURL] = url;
                request.Method = "POST";
                request.Headers.Add("X-WNS-RequestForStatus", "true");

                request.ContentLength = contentInBytes.Length;
                switch (NotififyType)
                {
                    case "toast":
                        request.Headers.Add("X-WNS-Type", "wns/toast");
                        request.ContentType = "text/xml";
                        break;
                    case "tile":
                        request.Headers.Add("X-WNS-Type", "wns/tile");
                        request.ContentType = "text/xml";
                        break;
                    case "raw":
                        request.Headers.Add("X-WNS-Type", "wns/raw");
                        request.ContentType = "application/octet-stream";
                        //application/octet-stream
                        //payload smaller than 5 KB in size.
                        break;
                    case "badge":
                        request.Headers.Add("X-WNS-Type", "wns/badge");
                        break;

                }

                //request.Headers.Add("X-WNS-Tag","msdn");

                request.Headers.Add("Authorization", String.Format("Bearer {0}", accessToken));

                using (Stream requestStream = request.GetRequestStream())
                    requestStream.Write(contentInBytes, 0, contentInBytes.Length);

                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                {
                    string state = webResponse.StatusCode.ToString();
                    StreamReader reader = new StreamReader(webResponse.GetResponseStream());
                    string result = reader.ReadToEnd();
                    string staus = webResponse.Headers["X-WNS-DeviceConnectionStatus"];


                    string X_WNS_DEVICECONNECTIONSTATUS = webResponse.Headers["X-WNS-DEVICECONNECTIONSTATUS"];
                    string X_WNS_NOTIFICATIONSTATUS = webResponse.Headers["X-WNS-NOTIFICATIONSTATUS"];
                    string X_WNS_STATUS = webResponse.Headers["X-WNS-STATUS"];
                    string X_WNS_MSG_ID = webResponse.Headers["X-WNS-MSG-ID"];
                    string X_WNS_DEBUG_TRACE = webResponse.Headers["X-WNS-DEBUG-TRACE"];
                    string Strict_Transport_Security = webResponse.Headers["Strict-Transport-Security"];
                    string Content_Length = webResponse.Headers["Content-Length"];
                    string Date = webResponse.Headers["Date"];

                    string webresponseheaders = "{" + "X-WNS-DEVICECONNECTIONSTATUS: " + X_WNS_DEVICECONNECTIONSTATUS + "\r\n"
                        + "X-WNS-NOTIFICATIONSTATUS: " + X_WNS_NOTIFICATIONSTATUS + "\r\n"
                        + "X-WNS-STATUS: " + X_WNS_STATUS + "\r\n"
                        + "X-WNS-MSG-ID: " + X_WNS_MSG_ID + "\r\n"
                        + "X-WNS-DEBUG-TRACE: " + X_WNS_DEBUG_TRACE + "\r\n"
                        + "Strict-Transport-Security: " + Strict_Transport_Security
                        + "Content-Length: " + Content_Length + "\r\n"
                        + "Date: " + Date
                        + "}";

                    steps[FlowSteps.StateCode] = webresponseheaders;

                    Debug.WriteLine(webresponseheaders);
                }
            }
            catch (WebException webException)
            {
                string msg = webException.Message;
                string X_WNS_Debug_Trace = webException.Response.Headers["X-WNS-Debug-Trace"];
                string X_WNS_Error_Description = webException.Response.Headers["X-WNS-Error-Description"];
                string X_WNS_Msg_ID = webException.Response.Headers["X-WNS-Msg-ID"];
                string X_WNS_Status = webException.Response.Headers["X-WNS-Status"];
                string debugOutput = "{" +"Exception Message: "+msg+"\r\n"
                    + "X-WNS-Debug-Trace: " + X_WNS_Debug_Trace + "\r\n"
                    + "X-WNS-Error-Description: " + X_WNS_Error_Description + "\r\n"
                    + "X-WNS-Msg-ID: " + X_WNS_Msg_ID + "\r\n"
                    + "X-WNS-Status: " + X_WNS_Status + "}";

                steps[FlowSteps.StateCode] = debugOutput;
            }
            return steps;
        }

        private OAuthToken GetOAuthTokenFromJson(string jsonString)
        {
            //using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            //{
            //    var ser = new DataContractJsonSerializer(typeof(OAuthToken));
            //    var oAuthToken = (OAuthToken)ser.ReadObject(ms);
            //    return oAuthToken;
            //}

            OAuthToken obj = JsonConvert.DeserializeObject<OAuthToken>(jsonString);
            return obj;

        }
    }
}
