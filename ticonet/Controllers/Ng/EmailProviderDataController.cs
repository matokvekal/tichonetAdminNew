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
using DEBS = Business_Logic.DictExpressionBuilderSystem;


namespace ticonet.Controllers.Ng
{
    public class EmailProviderDataController : NgController<EmailSenderDataProviderVM> {

        protected override NgResult _create(EmailSenderDataProviderVM[] models) {
            using (var l = new MessagesModuleLogic()) {
                foreach (var model in models) {
                    var item = l.Create<tblEmailSenderDataProvider>();
                    EmailSenderDataProviderVM.ReflectToTblEmailSenderDataProvider.Run(model, item);
                    l.Add(item);
                }
            }
            return NgResult.Succes(DEBS.Translate("MessageMdl.{0} email sms providers was added", models.Count())); ;
        }

        protected override NgResult _delete(EmailSenderDataProviderVM[] models) {
            using (var l = new MessagesModuleLogic()) {
                foreach (var model in models) {
                    l.Delete<tblEmailSenderDataProvider>(model.Id);
                }
            }
            return NgResult.Succes(DEBS.Translate("MessageMdl.{0} email providers was removed", models.Count()));
        }

        protected override FetchResult<EmailSenderDataProviderVM> _fetch(int? Skip, int? Count, NgControllerInstruct[] filters) {
            using (var l = new MessagesModuleLogic()) {
                int allQCount;
                var queryResult = l.GetFiltered<tblEmailSenderDataProvider>(Skip, Count, filters, out allQCount)
                    .Select(x => PocoConstructor.MakeFromObj(x, EmailSenderDataProviderVM.tblEmailSenderDataProviderPR));
                return FetchResult<EmailSenderDataProviderVM>.Succes(queryResult, allQCount);
            }
        }

        protected override NgResult _update(EmailSenderDataProviderVM[] models) {
            using (var l = new MessagesModuleLogic()) {
                foreach (var model in models) {
                    var item = l.Get<tblEmailSenderDataProvider>(model.Id);
                    EmailSenderDataProviderVM.ReflectToTblEmailSenderDataProvider.Run(model, item);
                    l.SaveChanges(item);
                }
            }
            return NgResult.Succes(DEBS.Translate("MessageMdl.{0} email providers was modified", models.Count()));
        }
    }
}