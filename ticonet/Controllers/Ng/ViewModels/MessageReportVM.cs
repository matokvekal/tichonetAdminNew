using Business_Logic.MessagesModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ticonet.Controllers.Ng.ViewModels {
    public class MessageReportVM : INgViewModel {
        public static POCOReflector<tblMessage, MessageReportVM> tblMessagePR =
             POCOReflector<tblMessage, MessageReportVM>.Create(
                 (o, m) => m.Id = o.Id,
                 (o, m) => m.Header = o.Header,
                 (o, m) => m.Body = o.Body,
                 (o, m) => m.Adress = o.Adress,
                 (o, m) => m.IsSms = o.IsSms,
                 (o, m) => m.SentOn = o.SentOn,
                 (o, m) => m.MessageBatchId = o.tblMessageBatchId,
                 (o, m) => m.ErrorLog = o.ErrorLog,
                 (o, m) => m.IsPending = o.tblPendingMessagesQueue != null && !o.tblPendingMessagesQueue.Deleted
            );

        public int Id { get; set; }

        public string Header { get; set; }
        public string Body { get; set; }
        public string Adress { get; set; }
        public bool IsSms { get; set; }
        public DateTime? SentOn { get; set; }
        public int MessageBatchId { get; set; }
        public string ErrorLog { get; set; }
        public bool IsPending { get; set; }

        #region INgViewModel

        public bool ng_JustCreated { get; set; }
        public bool ng_ToDelete { get; set; }

        #endregion
    }
}