using System.Net;
using System.Net.Mail;
using System.Web.Configuration;
using System.Collections.Generic;
using Business_Logic.MessagesModule.EntitiesExtensions;
using System.Linq;
using System;
using System.Text;

namespace Business_Logic.MessagesModule.Mechanisms {

    public interface IMessageSender <TSendServiceProvider,TMessage>
        where TSendServiceProvider : ISendServiceProvider
        where TMessage: IMessage {

        void SendBatch(IEnumerable<TMessage> messages, TSendServiceProvider provider);
        void SendSingle(TMessage message, TSendServiceProvider provider); 
    }

    public class EmailSender : BatchSendingComponent, IMessageSender<IEmailServiceProvider, IEmailMessage> {

        public EmailSender(BatchSendingManager manager) : base(manager) {
        }

        public void SendBatch(IEnumerable<IEmailMessage> messages, IEmailServiceProvider provider) {
            OpenSmptAndDO(provider, smtp => {
                foreach (var msg in messages)
                    SendEmail(msg, provider, smtp);
            });
        }

        public void SendSingle(IEmailMessage msg, IEmailServiceProvider provider) {
            OpenSmptAndDO(provider, smtp => SendEmail(msg, provider, smtp));
        }

        void SendEmail(IEmailMessage msg, IEmailServiceProvider provider, SmtpClient smtp) {
            try
            {
                string displayName = !string.IsNullOrWhiteSpace(msg.RecepientName) ? msg.RecepientName : string.Empty;
                var toAddress = new MailAddress(msg.RecepientAdress, msg.RecepientName);

                StringBuilder msgBody = new StringBuilder();
                msgBody.Append("<body style=\"direction:rtl;font-family:Arial\">");
                msgBody.Append(msg.Body);
                msgBody.Append("</body>");
                    using (var message = new MailMessage(provider.FromEmailAddress, toAddress)
                    {
                        Subject = msg.Subject,
                        Body = msgBody.ToString(),
                        IsBodyHtml = msg.IsBodyHtml
                    })
                    {
                        smtp.Send(message);
                        msg.SendDate = DateTime.Now;
                    }
            }
            catch (SmtpException e)
            {
                msg.AddError("SmtpException: " + e.Message);
            }
        }

        void OpenSmptAndDO(IEmailServiceProvider provider, Action<SmtpClient> action) {
            using (var smtp = new SmtpClient {
                Host = provider.SmtpHostName,
                Port = provider.SmtpPort,
                EnableSsl = provider.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = provider.NetworkCredentials
            })
                action(smtp);
        }
    }
}