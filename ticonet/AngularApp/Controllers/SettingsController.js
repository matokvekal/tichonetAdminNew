var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var AngularApp;
(function (AngularApp) {
    var Controllers;
    (function (Controllers) {
        var SettingsVA = (function () {
            function SettingsVA() {
                this.batchSendingIsActive = false;
                this.smsProvs = [];
                this.emailProvs = [];
            }
            return SettingsVA;
        }());
        var SettingsController = (function (_super) {
            __extends(SettingsController, _super);
            function SettingsController($rootScope, $scope, $http) {
                var _this = this;
                _super.call(this, $rootScope, $scope, $http);
                this.initData = function () {
                    _this.fetchtoarr(true, { urlalias: "getsmsprovs" }, _this.va.smsProvs, true);
                    _this.fetchtoarr(true, { urlalias: "getemailprovs" }, _this.va.emailProvs, true);
                };
                this.saveProvider = function (prov, callback) {
                    var mode = prov.ng_JustCreated ? "cr" : "up";
                    //let callback = prov.ng_JustCreated ? (r) => this.initData() : (r) => { }
                    var alias = Controllers.IsEmailSenderDataProviderVM(prov) ? "mngemailprovs" : "mngsmsprovs";
                    _this.request(true, {
                        urlalias: alias, params: { models: [prov], mode: mode },
                        onSucces: callback
                    });
                };
                this.deleteProvider = function (prov) {
                    var isEmailProv = Controllers.IsEmailSenderDataProviderVM(prov);
                    var alias = isEmailProv ? "mngemailprovs" : "mngsmsprovs";
                    var arr = isEmailProv ? _this.va.emailProvs : _this.va.smsProvs;
                    _this.request(true, {
                        urlalias: alias,
                        params: { models: [prov], mode: "dl" },
                        onSucces: function (r) { return arr.remove(prov); }
                    });
                };
                this.createProvider = function (SmsProvider) {
                    var newProv;
                    if (SmsProvider)
                        newProv = new Controllers.SmsSenderDataProviderVM();
                    else
                        newProv = new Controllers.EmailSenderDataProviderVM();
                    var cb = function (r) {
                        _this.initData();
                        //newProv.ng_JustCreated = false
                        //if (SmsProvider) this.va.smsProvs.unshift(newProv as SmsSenderDataProviderVM)
                        //else this.va.emailProvs.unshift(newProv as EmailSenderDataProviderVM)
                    };
                    _this.saveProvider(newProv, cb);
                };
                this.refetchBatchSendingIsActive = function (isActive) {
                    //TODO Update config
                };
            }
            SettingsController.prototype.buildVa = function () { return new SettingsVA; };
            SettingsController.prototype.init = function (data) {
                var _this = this;
                //------------------- RequestMsgs
                this.request_msgHandlerSucces = function (msg) {
                    _this.ShowNotification("Info", msg, { glicon: "info-sign", nclass: "info" }, 3000);
                };
                this.request_msgHandlerFail = function (msg) {
                    _this.ShowNotification("Error", msg, { glicon: "ban-circle", nclass: "error" });
                };
                //translator 
                this.rootScope.$broadcast('Give_Me_Translator', { postBack: function (T) { return _this.Translator = T; } });
                //Scope Init
                this.scope.SaveProvider = this.saveProvider;
                this.scope.DeleteProvider = this.deleteProvider;
                this.scope.CreateSmsProvider = function () { return _this.createProvider(true); };
                this.scope.CreateEmailProvider = function () { return _this.createProvider(false); };
                this.scope.SetBatchSendingIsActive = function (isActive) {
                    _this.refetchBatchSendingIsActive(isActive);
                };
                //Inner Init
                this.initUrlModuleFromRowObj(data.urls);
                this.initData();
            };
            return SettingsController;
        }(AngularApp.Controller));
        Controllers.SettingsController = SettingsController;
    })(Controllers = AngularApp.Controllers || (AngularApp.Controllers = {}));
})(AngularApp || (AngularApp = {}));
