var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var AngularApp;
(function (AngularApp) {
    var Controllers;
    (function (Controllers) {
        var tru = TranslatonUtils;
        var fnc = TSNetLike.Functors;
        var PageBlocksVA = (function () {
            function PageBlocksVA() {
                this.filters_folded = true;
                this.templates_folded = true;
                this.mails_folded = true;
                this.reports_folded = true;
            }
            return PageBlocksVA;
        }());
        var MessageModulePageController = (function (_super) {
            __extends(MessageModulePageController, _super);
            function MessageModulePageController($rootScope, $scope, $http) {
                _super.call(this, $rootScope, $scope, $http);
                this.TurnHoldViewOnOthersControllers();
            }
            MessageModulePageController.prototype.buildVa = function () { return new PageBlocksVA; };
            MessageModulePageController.prototype.init = function (data) {
                var _this = this;
                data = data || { translations: { "it {0} work": "it will {0} work" } };
                this.Translator = new tru.Translator(data.translations);
                this.rootScope.$on('Give_Me_Translator', function (event, args) {
                    fnc.F(args.postBack, _this.Translator);
                });
            };
            return MessageModulePageController;
        }(AngularApp.Controller));
        Controllers.MessageModulePageController = MessageModulePageController;
    })(Controllers = AngularApp.Controllers || (AngularApp.Controllers = {}));
})(AngularApp || (AngularApp = {}));
