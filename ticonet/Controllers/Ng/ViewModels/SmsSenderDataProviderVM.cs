using Business_Logic.MessagesModule;
using Business_Logic.MessagesModule.DataObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ticonet.Controllers.Ng.ViewModels {
    public class SmsSenderDataProviderVM : INgViewModel {
        public static POCOReflector<tblSmsSenderDataProvider, SmsSenderDataProviderVM> tblSmsSenderDataProviderPR =
             POCOReflector<tblSmsSenderDataProvider, SmsSenderDataProviderVM>.Create(
                 (o, m) => m.Id = o.Id,
                 (o, m) => m.Name = o.Name,
                 (o, m) => m.IsActive = o.IsActive,
                 (o, m) => m.FromDisplayName = o.FromDisplayName,
                 (o, m) => m.FromPhoneNumber = o.FromPhoneNumber,
                 (o, m) => m.Username = o.Username,
                 (o, m) => m.Password = o.Password,
                 (o, m) => m.MessageInterval = o.MessageInterval,
                 (o, m) => m.RestrictionData = JsonConvert.DeserializeObject<SendProviderRestrictionData>(o.SendProviderRestrictionDataJSON)
            );

        public static POCOReflector<SmsSenderDataProviderVM, tblSmsSenderDataProvider> ReflectToTblSmsSenderDataProvider =
             POCOReflector<SmsSenderDataProviderVM, tblSmsSenderDataProvider>.Create(
                 (m, o) => o.Name = m.Name ?? string.Empty,
                 (m, o) => o.IsActive = m.IsActive,
                 (m, o) => o.FromDisplayName = m.FromDisplayName ?? string.Empty,
                 (m, o) => o.FromPhoneNumber = m.FromPhoneNumber ?? string.Empty,
                 (m, o) => o.Username = m.Username ?? string.Empty,
                 (m, o) => o.Password = m.Password ?? string.Empty,
                 (m, o) => o.MessageInterval = m.MessageInterval,
                 (m, o) => o.SendProviderRestrictionDataJSON = JsonConvert.SerializeObject(m.RestrictionData)
            );


        public int Id { get; set; }

        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string FromDisplayName { get; set; }
        public string FromPhoneNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int MessageInterval { get; set; }
        public SendProviderRestrictionData RestrictionData { get; set; }

        #region INgViewModel

        public bool ng_JustCreated { get; set; }
        public bool ng_ToDelete { get; set; }

        #endregion
    }
}