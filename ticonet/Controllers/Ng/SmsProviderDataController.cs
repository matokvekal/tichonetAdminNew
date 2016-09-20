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


namespace ticonet.Controllers.Ng {
    public class SmsProviderDataController : NgController<SmsSenderDataProviderVM> {
        protected override NgResult _create(SmsSenderDataProviderVM[] models) {
            using (var l = new MessagesModuleLogic()) {
                foreach (var model in models) {
                    var item = l.Create<tblSmsSenderDataProvider>();
                    SmsSenderDataProviderVM.ReflectToTblSmsSenderDataProvider.Run(model, item);
                    l.Add(item);
                }
            }
            return NgResult.Succes(DEBS.Translate("MessageMdl.{0} new sms providers was added", models.Count())); ;
        }

        protected override NgResult _delete(SmsSenderDataProviderVM[] models) {
            using (var l = new MessagesModuleLogic()) {
                foreach (var model in models) {
                    l.Delete<tblSmsSenderDataProvider>(model.Id);
                }
            }
            return NgResult.Succes(DEBS.Translate("MessageMdl.{0} sms providers was removed", models.Count()));
        }

        protected override FetchResult<SmsSenderDataProviderVM> _fetch(int? Skip, int? Count, NgControllerInstruct[] filters) {
            using (var l = new MessagesModuleLogic()) {
                int allQCount;
                var queryResult = l.GetFiltered<tblSmsSenderDataProvider>(Skip, Count, filters, out allQCount)
                    .Select(x => PocoConstructor.MakeFromObj(x, SmsSenderDataProviderVM.tblSmsSenderDataProviderPR));
                return FetchResult<SmsSenderDataProviderVM>.Succes(queryResult, allQCount);
            }
        }

        protected override NgResult _update(SmsSenderDataProviderVM[] models) {
            using (var l = new MessagesModuleLogic()) {
                foreach (var model in models) {
                    var item = l.Get<tblSmsSenderDataProvider>(model.Id);
                    SmsSenderDataProviderVM.ReflectToTblSmsSenderDataProvider.Run(model, item);
                    l.SaveChanges(item);
                }
            }
            return NgResult.Succes(DEBS.Translate("MessageMdl.{0} sms providers was modified", models.Count()));
        }
    }
}