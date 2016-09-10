using Business_Logic.MessagesModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ticonet.Controllers.Ng.ViewModels {
    public class BatchReportVM : INgViewModel {
        public static POCOReflector<tblMessageBatch, BatchReportVM> tblMessageBatchPR =
             POCOReflector<tblMessageBatch, BatchReportVM>.Create(
                 (o, m) => m.Id = o.Id,
                 (o, m) => m.BaseScheduleName = o.tblMessageSchedule.Name,
                 (o, m) => m.CreatedOn = o.CreatedOn,
                 (o, m) => m.FinishedOn = o.FinishedOn,
                 (o, m) => m.Errors = o.Errors,
                 (o, m) => m.IsSms = o.IsSms,
                 (o, m) => m.MessagesCount = o.tblMessages.Count()
            );

        public int Id { get; set; }

        public string BaseScheduleName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? FinishedOn { get; set; }
        public string Errors { get; set; }
        public bool IsSms { get; set; }
        public int MessagesCount { get; set; }

        #region INgViewModel

        public bool ng_JustCreated { get; set; }
        public bool ng_ToDelete { get; set; }

        #endregion
    }
}