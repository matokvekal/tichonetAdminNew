var AngularApp;
(function (AngularApp) {
    var Controllers;
    (function (Controllers) {
        function FindById(arr, Id) {
            return arr.first(function (x) { return x.Id === Id; });
        }
        Controllers.FindById = FindById;
        var BaseTableVM = (function () {
            function BaseTableVM() {
            }
            return BaseTableVM;
        }());
        Controllers.BaseTableVM = BaseTableVM;
        var MetaFilterVM = (function () {
            function MetaFilterVM() {
                this.filters = [];
                this.wildcards = [];
                this.reccards = [];
                this.Name = "New Filter";
                this.BaseTableId = -1;
                this.Invalid = false;
            }
            return MetaFilterVM;
        }());
        Controllers.MetaFilterVM = MetaFilterVM;
        var KeyVM = (function () {
            function KeyVM() {
            }
            return KeyVM;
        }());
        Controllers.KeyVM = KeyVM;
        var ValOp = (function () {
            function ValOp() {
                this.Value = "";
                this.Operator = "=";
            }
            return ValOp;
        }());
        Controllers.ValOp = ValOp;
        var FilterVM = (function () {
            function FilterVM() {
                //local
                this.Invalid = false;
            }
            return FilterVM;
        }());
        Controllers.FilterVM = FilterVM;
        var FilterValueContainer = (function () {
            function FilterValueContainer() {
                this.Values = [];
            }
            return FilterValueContainer;
        }());
        Controllers.FilterValueContainer = FilterValueContainer;
        var WildcardVM = (function () {
            function WildcardVM() {
                this.Invalid = false;
            }
            return WildcardVM;
        }());
        Controllers.WildcardVM = WildcardVM;
        var RecepientCardVM = (function () {
            function RecepientCardVM() {
                //local
                this.Invalid = false;
            }
            return RecepientCardVM;
        }());
        Controllers.RecepientCardVM = RecepientCardVM;
        var FiltOperator = (function () {
            function FiltOperator() {
            }
            return FiltOperator;
        }());
        Controllers.FiltOperator = FiltOperator;
        var TemplateVM = (function () {
            function TemplateVM() {
                this.RecepientFilterId = -1;
                this.Name = "New Template";
                this.FilterValueContainers = [];
                this.ChoosenReccards = [];
            }
            return TemplateVM;
        }());
        Controllers.TemplateVM = TemplateVM;
        var MessageScheduleVM = (function () {
            function MessageScheduleVM() {
                this.TemplateId = -1;
                this.Name = "New Template";
                this.FilterValueContainers = [];
                this.ChoosenReccards = [];
            }
            return MessageScheduleVM;
        }());
        Controllers.MessageScheduleVM = MessageScheduleVM;
        var BatchReportVM = (function () {
            function BatchReportVM() {
            }
            BatchReportVM.ServerDataMarshall = function (data) {
                data.CreatedOn = Controllers.formatVal(data.CreatedOn, "datetime");
                data.FinishedOn = Controllers.formatVal(data.FinishedOn, "datetime");
                return data;
            };
            return BatchReportVM;
        }());
        Controllers.BatchReportVM = BatchReportVM;
        var MessageReportVM = (function () {
            function MessageReportVM() {
            }
            return MessageReportVM;
        }());
        Controllers.MessageReportVM = MessageReportVM;
        var SendProviderRestrictionData = (function () {
            function SendProviderRestrictionData() {
                this.MaxMessagesInHour = 1;
                this.MaxMessagesInDay = 1;
            }
            return SendProviderRestrictionData;
        }());
        Controllers.SendProviderRestrictionData = SendProviderRestrictionData;
        function IsEmailSenderDataProviderVM(prov) {
            return typeof prov.EnableSsl !== 'undefined';
        }
        Controllers.IsEmailSenderDataProviderVM = IsEmailSenderDataProviderVM;
        var EmailSenderDataProviderVM = (function () {
            function EmailSenderDataProviderVM() {
                this.Id = -1;
                this.Name = "NewEmailServiceProvider";
                this.IsActive = false;
                this.FromEmailAddress = "from@mail.com";
                this.FromEmailDisplayName = "FromName";
                this.FromEmailPassword = "";
                this.SmtpHostName = "";
                this.SmtpPort = 25;
                this.EnableSsl = true;
                this.RestrictionData = new SendProviderRestrictionData();
                this.ng_JustCreated = true;
                this.ng_ToDelete = false;
            }
            return EmailSenderDataProviderVM;
        }());
        Controllers.EmailSenderDataProviderVM = EmailSenderDataProviderVM;
        var SmsSenderDataProviderVM = (function () {
            function SmsSenderDataProviderVM() {
                this.Id = -1;
                this.Name = "NewSmsServiceProvider";
                this.IsActive = false;
                this.FromDisplayName = "FromName";
                this.FromPhoneNumber = "000";
                this.Username = "username";
                this.Password = "";
                this.MessageInterval = 5;
                this.RestrictionData = new SendProviderRestrictionData();
                this.ng_JustCreated = true;
                this.ng_ToDelete = false;
            }
            return SmsSenderDataProviderVM;
        }());
        Controllers.SmsSenderDataProviderVM = SmsSenderDataProviderVM;
    })(Controllers = AngularApp.Controllers || (AngularApp.Controllers = {}));
})(AngularApp || (AngularApp = {}));
//# sourceMappingURL=SharedViewModels.js.map