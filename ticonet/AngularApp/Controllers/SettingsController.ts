namespace AngularApp.Controllers {
    import fnc = TSNetLike.Functors

    class SettingsVA {
        batchSendingIsActive: boolean = false
    }

    export class SettingsController extends Controller<SettingsVA> {
        constructor($rootScope, $scope, $http) {
            super($rootScope, $scope, $http)
        }
        buildVa(): SettingsVA { return new SettingsVA }
        init(data): void {
            
            this.scope.SetBatchSendingIsActive = (isActive: boolean) => {
                this.refetchBatchSendingIsActive(isActive);
            }
        }

        refetchBatchSendingIsActive = (isActive: boolean) => {
            //TODO Update config
        }

    }


}