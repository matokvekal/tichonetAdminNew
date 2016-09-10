namespace AngularApp.Controllers {
    import col = TSNetLike.Collections

    class ReportVA {
        batches: GridManager<BatchReportVM>
        messages: GridManager<MessageReportVM>
        curbatch: BatchReportVM = null
    }

    export class ReportsController extends Controller<ReportVA> {
        private Translator: TranslatonUtils.Translator

        constructor($rootScope, $scope, $http) {
            super($rootScope, $scope, $http)
        }
        buildVa(): ReportVA { return new ReportVA }
        init(data): void {
            //------------------- RequestMsgs
            this.request_msgHandlerSucces = (msg) => {
                this.ShowNotification("Info", msg, { glicon: "info-sign", nclass: "info" }, 3000)
            }
            this.request_msgHandlerFail = (msg) => {
                this.ShowNotification("Error", msg, { glicon: "ban-circle", nclass: "error" })
            }

            //translator 
            this.rootScope.$broadcast('Give_Me_Translator', { postBack: (T) => this.Translator = T })

            //------------------- Scope Init

            this.scope.LoadMessages = (Batch: BatchReportVM) => {
                this.va.messages.SetSettingValue("BatchId",Batch.Id).Reset().Refetch()
            }

            this.scope.UnloadMessages = () => {
                this.va.messages.Clear()
            }

            //------------------- Inner Init

            this.initUrlModuleFromRowObj(data.urls)

            this.va.batches = new GridManager<BatchReportVM>(this, "getbatches")
                .AddSetting("IsSms", false, (fp, val) => fp.addFilt("IsSms", val, "="),
                    (val, settings) => settings.IsSms !== settings.IsEmail)
                .AddSetting("IsEmail", false, (fp, val) => fp.addFilt("IsSms", val, "<>"),
                    (val, settings) => settings.IsSms !== settings.IsEmail)
                .Marshaller(x => BatchReportVM.ServerDataMarshall(x))
                .DefaultPagination(20)

            this.va.messages = new GridManager<MessageReportVM>(this, "getmessages")
                .AddSetting("BatchId", 0, (fp, val) => fp.addFilt("tblMessageBatchId", val, "="))
                .DefaultPagination(100)

            this.va.batches.Reset().Refetch()

        }
    }

}