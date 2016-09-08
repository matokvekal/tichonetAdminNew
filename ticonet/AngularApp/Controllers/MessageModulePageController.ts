namespace AngularApp.Controllers {
    import col = TSNetLike.Collections
    import tru = TranslatonUtils
    import fnc = TSNetLike.Functors

    class PageBlocksVA {
        filters_folded = true
        templates_folded = true
        mails_folded = true
        reports_folded = true
    }

    export class MessageModulePageController extends Controller<PageBlocksVA> {
        constructor($rootScope, $scope, $http) {
            super($rootScope, $scope, $http)
            this.TurnHoldViewOnOthersControllers()
        }
        Translator: tru.Translator

        buildVa(): PageBlocksVA { return new PageBlocksVA }
        init(data): void {
            data = data || { translations: { "it {0} work": "it will {0} work"}}
            this.Translator = new tru.Translator(data.translations)
            this.rootScope.$on('Give_Me_Translator', (event, args) => {
                fnc.F(args.postBack,this.Translator)
            });
        }
    }


}