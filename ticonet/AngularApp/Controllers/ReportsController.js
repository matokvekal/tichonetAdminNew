var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var AngularApp;
(function (AngularApp) {
    var Controllers;
    (function (Controllers) {
        var ReportVA = (function () {
            function ReportVA() {
                this.curbatch = null;
            }
            return ReportVA;
        }());
        var ReportsController = (function (_super) {
            __extends(ReportsController, _super);
            function ReportsController($rootScope, $scope, $http) {
                _super.call(this, $rootScope, $scope, $http);
            }
            ReportsController.prototype.buildVa = function () { return new ReportVA; };
            ReportsController.prototype.init = function (data) {
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
                //------------------- Scope Init
                this.scope.LoadMessages = function (Batch) {
                    _this.va.messages.SetSettingValue("BatchId", Batch.Id).Reset().Refetch();
                };
                this.scope.UnloadMessages = function () {
                    _this.va.messages.Clear();
                };
                //------------------- Inner Init
                this.initUrlModuleFromRowObj(data.urls);
                this.va.batches = new AngularApp.GridManager(this, "getbatches")
                    .AddSetting("IsSms", false, function (fp, val) { return fp.addFilt("IsSms", val, "="); }, function (val, settings) { return settings.IsSms !== settings.IsEmail; })
                    .AddSetting("IsEmail", false, function (fp, val) { return fp.addFilt("IsSms", val, "<>"); }, function (val, settings) { return settings.IsSms !== settings.IsEmail; })
                    .Marshaller(function (x) { return Controllers.BatchReportVM.ServerDataMarshall(x); })
                    .DefaultPagination(20);
                this.va.messages = new AngularApp.GridManager(this, "getmessages")
                    .AddSetting("BatchId", 0, function (fp, val) { return fp.addFilt("tblMessageBatchId", val, "="); })
                    .DefaultPagination(100);
                this.va.batches.Reset().Refetch();
            };
            return ReportsController;
        }(AngularApp.Controller));
        Controllers.ReportsController = ReportsController;
    })(Controllers = AngularApp.Controllers || (AngularApp.Controllers = {}));
})(AngularApp || (AngularApp = {}));
//# sourceMappingURL=ReportsController.js.map