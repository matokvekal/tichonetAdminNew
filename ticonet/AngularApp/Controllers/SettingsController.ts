namespace AngularApp.Controllers {
    import fnc = TSNetLike.Functors

    class SettingsVA {
        batchSendingIsActive: boolean = false
        smsProvs: SmsSenderDataProviderVM[] = []
        emailProvs: EmailSenderDataProviderVM[] = []
    }

    export class SettingsController extends Controller<SettingsVA> {
        private Translator: TranslatonUtils.Translator

        constructor($rootScope, $scope, $http) {
            super($rootScope, $scope, $http)
        }
        buildVa(): SettingsVA { return new SettingsVA }
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

            //Scope Init

            this.scope.SaveProvider = this.saveProvider
            this.scope.DeleteProvider = this.deleteProvider

            this.scope.SetBatchSendingIsActive = (isActive: boolean) => {
                this.refetchBatchSendingIsActive(isActive);
            }

            //Inner Init
            this.initUrlModuleFromRowObj(data.urls)
            this.initData()
        }

        initData = () => {
            this.fetchtoarr(true, { urlalias: "getsmsprovs" }, this.va.smsProvs, true)
            this.fetchtoarr(true, { urlalias: "getemailprovs" }, this.va.emailProvs, true)
        }

        saveProvider = (prov: EmailSenderDataProviderVM | SmsSenderDataProviderVM) => {
            let mode = prov.ng_JustCreated ? "cr" : "up"
            let alias = IsEmailSenderDataProviderVM(prov) ? "mngemailprovs" : "mngsmsprovs"
            this.request(true, { urlalias: alias, params: { models: [prov], mode: mode } } )
        }

        deleteProvider = (prov: EmailSenderDataProviderVM | SmsSenderDataProviderVM) => {
            let isEmailProv = IsEmailSenderDataProviderVM(prov)
            let alias = isEmailProv ? "mngemailprovs" : "mngsmsprovs"
            let arr = isEmailProv ? this.va.emailProvs : this.va.smsProvs
            this.request(true, {
                urlalias: alias,
                params: { models: [prov], mode: "dl" },
                onSucces: (r) => (arr as any).remove(prov)
            })
        }

        createProvider = (SmsProvider: boolean) => {
            if (SmsProvider)
                this.va.smsProvs.unshift(new SmsSenderDataProviderVM())
            else
                this.va.emailProvs.unshift(new EmailSenderDataProviderVM())
        }

        refetchBatchSendingIsActive = (isActive: boolean) => {
            //TODO Update config
        }

    }


}