using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business_Logic.MessagesModule.DataObjects;
using Business_Logic.MessagesModule;
using Newtonsoft.Json;

//ReflectToTblTemplate
namespace ticonet.Controllers.Ng.ViewModels {
    public class EmailSenderDataProviderVM : INgViewModel {
        public static POCOReflector<tblEmailSenderDataProvider, EmailSenderDataProviderVM> tblEmailSenderDataProviderPR =
             POCOReflector<tblEmailSenderDataProvider, EmailSenderDataProviderVM>.Create(
                 (o, m) => m.Id = o.Id,
                 (o, m) => m.Name = o.Name,
                 (o, m) => m.IsActive = o.IsActive,
                 (o, m) => m.FromEmailAddress = o.FromEmailAddress,
                 (o, m) => m.FromEmailDisplayName = o.FromEmailDisplayName,
                 (o, m) => m.FromEmailPassword = o.FromEmailPassword,
                 (o, m) => m.SmtpHostName = o.SmtpHostName,
                 (o, m) => m.SmtpPort = o.SmtpPort,
                 (o, m) => m.EnableSsl = o.EnableSsl,
                 (o, m) => m.RestrictionData = JsonConvert.DeserializeObject<SendProviderRestrictionData>(o.SendProviderRestrictionDataJSON)
            );

        public static POCOReflector<EmailSenderDataProviderVM, tblEmailSenderDataProvider> ReflectToTblEmailSenderDataProvider =
             POCOReflector<EmailSenderDataProviderVM, tblEmailSenderDataProvider>.Create(
                 (m, o) => o.Name = m.Name,
                 (m, o) => o.IsActive = m.IsActive,
                 (m, o) => o.FromEmailAddress = m.FromEmailAddress,
                 (m, o) => o.FromEmailDisplayName = m.FromEmailDisplayName,
                 (m, o) => o.FromEmailPassword = m.FromEmailPassword,
                 (m, o) => o.SmtpHostName = m.SmtpHostName,
                 (m, o) => o.SmtpPort = m.SmtpPort,
                 (m, o) => o.EnableSsl = m.EnableSsl,
                 (m, o) => o.SendProviderRestrictionDataJSON = JsonConvert.SerializeObject(m.RestrictionData)
            );


        public int Id { get; set; }

        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string FromEmailAddress { get; set; }
        public string FromEmailDisplayName { get; set; }
        public string FromEmailPassword { get; set; }
        public string SmtpHostName { get; set; }
        public int SmtpPort { get; set; }
        public bool EnableSsl { get; set; }
        public SendProviderRestrictionData RestrictionData { get; set; }

        #region INgViewModel

        public bool ng_JustCreated { get; set; }
        public bool ng_ToDelete { get; set; }

        #endregion
    }
}