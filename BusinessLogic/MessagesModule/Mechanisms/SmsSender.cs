using Business_Logic.MessagesModule.EntitiesExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Business_Logic.MessagesModule.Mechanisms {
    public class SmsSender : BatchSendingComponent, IMessageSender<ISmsServiceProvider, ISmsMessage> {

        public SmsSender(BatchSendingManager manager) : base(manager) {
        }

        public void SendBatch(IEnumerable<ISmsMessage> messages, ISmsServiceProvider provider) {
            foreach (var msg in messages)
                Send(msg, provider);
        }

        public void SendSingle(ISmsMessage message, ISmsServiceProvider provider) {
            Send(message, provider);
        }

        //public interface ISmsServiceProvider : ISendServiceProvider {
        //    //string SmtpHostName { get; }
        //    //int SmtpPort { get; }
        //    //bool EnableSsl { get; }

        //    //MailAddress FromEmailAddress { get; }
        //    //NetworkCredential NetworkCredentials { get; }
        //}

        void Send(ISmsMessage Msg, ISmsServiceProvider Provider) {
            try {
                string messageText = SecurityElement.Escape(Msg.Text);
                StringBuilder sbXml = new StringBuilder();
                sbXml.Append("<Inforu>");
                sbXml.Append("<User>");
                sbXml.Append("<Username>" + Provider.UserName + "</Username>");
                sbXml.Append("<Password>" + Provider.Password + "</Password>");
                sbXml.Append("</User>");
                sbXml.Append("<Content Type=\"sms\">");
                sbXml.Append("<Message>" + messageText + "</Message>");
                sbXml.Append("</Content>");
                sbXml.Append("<Recipients>");
                sbXml.Append("<PhoneNumber>" + Msg.PhoneNumber + "</PhoneNumber>");
                sbXml.Append("</Recipients>");
                sbXml.Append("<Settings>");
                sbXml.Append("<SenderName>" + Provider.SenderName + "</SenderName>");
                sbXml.Append("<SenderNumber>" + Provider.SenderNumber + "</SenderNumber>");
                sbXml.Append("<MessageInterval>" + Provider.MessageInterval + "</MessageInterval>");
                sbXml.Append("</Settings>");
                sbXml.Append("</Inforu >");
                string strXML = HttpUtility.UrlEncode(sbXml.ToString(), Encoding.UTF8);
                //todo await result ?
                Task<string> x = PostDataToURL("http://api.smsim.co.il/SendMessageXml.ashx", "InforuXML=" + strXML);
                Msg.SendDate = DateTime.Now;
            }
            catch (Exception exc) {
                Msg.AddError("SmsServiceException: " + exc.Message);
            }
        }

        static async Task<string> PostDataToURL(string szUrl, string szData) {
            //Setup the web request
            string szResult = string.Empty;
            WebRequest Request = WebRequest.Create(szUrl);
            Request.Timeout = 30000;
            Request.Method = "POST";
            Request.ContentType = "application/x-www-form-urlencoded";
            //Set the POST data in a buffer
            byte[] PostBuffer;
            try {
                // replacing " " with "+" according to Http post RPC
                szData = szData.Replace(" ", "+");
                //Specify the length of the buffer
                PostBuffer = Encoding.UTF8.GetBytes(szData);
                Request.ContentLength = PostBuffer.Length;

                Task<string> t = new Task<string>(() => {
                    //Open up a request stream
                    Stream RequestStream = Request.GetRequestStream();
                    //Write the POST data
                    RequestStream.Write(PostBuffer, 0, PostBuffer.Length);
                    //Close the stream
                    RequestStream.Close();
                    //Create the Response object
                    WebResponse Response;
                    Response = Request.GetResponse();
                    //Create the reader for the response
                    StreamReader sr = new StreamReader(Response.GetResponseStream(), Encoding.UTF8);
                    //Read the response
                    szResult = sr.ReadToEnd();
                    //Close the reader, and response
                    sr.Close();
                    Response.Close();
                    return szResult;
                });
                t.Start();
                string result = await t;
                return result;
            }
            catch {
                return szResult;
            }
        }
    }
}
