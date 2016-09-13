using System;
using System.Linq;
using ticonet.ParentControllers;
using ticonet.Controllers.Ng.ViewModels;
using Business_Logic.MessagesModule;
using System.Web.Mvc;
using Ninject;
using Business_Logic.SqlContext;
using Business_Logic.MessagesModule.Mechanisms;
using Business_Logic.MessagesModule.DataObjects;

namespace ticonet.Controllers.Ng {

    [Authorize]
    public class BatchReportsController : NgController<BatchReportVM> {

        protected override NgResult _create(BatchReportVM[] models) {
            throw new NotSupportedException();
        }

        protected override NgResult _delete(BatchReportVM[] models) {
            throw new NotSupportedException();
        }

        protected override FetchResult<BatchReportVM> _fetch(int? Skip, int? Count, NgControllerInstruct[] filters) {
            using (var l = new MessagesModuleLogic()) {
                var query = l.GetFilteredQueryable<tblMessageBatch>(filters);
                int count = query.Count();
                query = query.OrderByDescending(x => x.FinishedOn).ThenByDescending(x => x.CreatedOn);
                if (Skip != null)
                    query = query.Skip(Skip.Value);
                if (Count != null)
                    query = query.Take(Count.Value);
                var queryResult = query.ToList().Select(x => PocoConstructor.MakeFromObj(x, BatchReportVM.tblMessageBatchPR));

                return FetchResult<BatchReportVM>.Succes(queryResult, count);
            }
        }

        protected override NgResult _update(BatchReportVM[] models) {
            throw new NotSupportedException();
        }
    }
}