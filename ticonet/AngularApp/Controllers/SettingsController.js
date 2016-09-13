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
            }
            return SettingsVA;
        }());
        var SettingsController = (function (_super) {
            __extends(SettingsController, _super);
            function SettingsController($rootScope, $scope, $http) {
                _super.call(this, $rootScope, $scope, $http);
                this.refetchBatchSendingIsActive = function (isActive) {
                    //TODO Update config
                };
            }
            SettingsController.prototype.buildVa = function () { return new SettingsVA; };
            SettingsController.prototype.init = function (data) {
                var _this = this;
                this.scope.SetBatchSendingIsActive = function (isActive) {
                    _this.refetchBatchSendingIsActive(isActive);
                };
            };
            return SettingsController;
        }(AngularApp.Controller));
        Controllers.SettingsController = SettingsController;
    })(Controllers = AngularApp.Controllers || (AngularApp.Controllers = {}));
})(AngularApp || (AngularApp = {}));
