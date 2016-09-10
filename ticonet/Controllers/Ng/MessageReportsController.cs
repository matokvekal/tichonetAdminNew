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
    public class MessageReportsController : NgController<MessageReportVM> {

        protected override NgResult _create(MessageReportVM[] models) {
            throw new NotSupportedException();
        }

        protected override NgResult _delete(MessageReportVM[] models) {
            throw new NotSupportedException();
        }

        protected override FetchResult<MessageReportVM> _fetch(int? Skip, int? Count, NgControllerInstruct[] filters) {
            using (var l = new MessagesModuleLogic()) {
                int count;
                var queryResult = l.GetFiltered<tblMessage>(Skip, Count, filters, out count)
                    .Select(x => PocoConstructor.MakeFromObj(x, MessageReportVM.tblMessagePR));
                return FetchResult<MessageReportVM>.Succes(queryResult, count);
            }
        }

        protected override NgResult _update(MessageReportVM[] models) {
            throw new NotSupportedException();
        }

    }
}