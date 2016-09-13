namespace AngularApp {
    import col = TSNetLike.Collections

    export class GridSetting {
        Name: string
        Default: any
        ParamMaker: (params: FetchParams, settingVal: any) => void
        Predicate: (settingVal: any, allSettings?: any) => boolean
    }

    export class GridManager<TModel>{
        constructor(protected Fetcher: IAjaxMaker, protected fetchUrlAlias: string) {
            this._settings = new col.Dictionary<GridSetting>()
        }

        Clear = () => this.Items = []

        IsNextPage = () => this.Take !== null || this.MaxQuery > this.Skip + this.Take
        IsPrevPage = () => this.Skip !== null || this.Skip !== 0

        Next = (skip?: number, take?: number) => {
            this.Skip = (this.Skip || 0) + (skip || this._pagination)
            this.Take = (take || this._pagination)
            return this;
        }

        Prev = (skip?: number, take?: number) => {
            this.Skip = (this.Skip || 0) - (skip || this._pagination)
            this.Take = (take || this._pagination)
            if (this.Skip < 0) this.Skip = 0
            return this;
        }

        Reset = (take?: number) => {
            this.Skip = 0
            this.Take = take || this._pagination
            return this
        }

        ResetSettings = () => {
            this._settings.iter((k, v) => {
                this.Settings[k] = v.Default
            })
            return this
        }

        Refetch = (background?: boolean, onSucces?: ((response?) => void) | ConcurentRequestHandler,
            onFailed?: ((response?) => void) | ConcurentRequestHandler) => {

            let args = this.BuildRequestArgs(onSucces, onFailed)
            this.Fetcher.Request(!background, args)
        }

        RefetchDelayed = (delayMilisecs: number, background?: boolean, onSucces?: ((response?) => void) | ConcurentRequestHandler,
            onFailed?: ((response?) => void) | ConcurentRequestHandler) => {

            this.Refetch(background, onSucces, onFailed)
        }

        protected BuildRequestArgs = (onSucces?: ((response?) => void) | ConcurentRequestHandler,
            onFailed?: ((response?) => void) | ConcurentRequestHandler) => {

            //Use this if you wrap onSucces/onFail here:
            //HandleCallBacks(onSucces, onFailed)
            //then after AJAX:
            //RunCallbackOrHandler(onSucces)
            //RunCallbackOrHandler(onFailed)

            HandleCallBacks(onSucces)

            let cb = (r) => {
                this.MaxQuery = r.data.allquerycount
                this.Items.splice(0, this.Items.length)

                let count = r.data.items.length
                r.data.items.forEach(x => this._fetchhandler(this._marshaller(x), this.Items, count, this.MaxQuery))

                RunCallbackOrHandler(onSucces, r)
            }

            let params = new FetchParams()
                .addSkip(this.Skip)
                .addCount(this.Take)

            this._settings.iter((k, v) => {
                let val = this.Settings[k]
                if (v.Predicate(val, this.Settings))
                    v.ParamMaker(params, val)
            })

            let args: IRequestArgs = {
                urlalias: this.fetchUrlAlias,
                onSucces: cb,
                onFailed: onFailed,
                params: params
            }
            return args
        }

        /**
        If no predicate passed, the predicate
        '(x) => typeof x !== "undefined"'
        will be used
        */
        AddSetting = (Name: string, Default: any,
            ParamMaker: (params: FetchParams, settingVal: any) => void,
            Predicate?: (settingVal: any, allSettings?: any) => boolean) => {

            this.Settings[Name] = Default
            this._settings.add(Name, {
                Name: Name,
                Default: Default,
                ParamMaker: ParamMaker,
                Predicate: Predicate || ((x) => typeof x !== 'undefined')
            })

            return this
        }

        SetSettingValue = (Name: string, Value: any) => {
            this[Name] = Value
            return this
        }

        Marshaller = (func: (item: TModel) => TModel) => {
            this._marshaller = func
            return this
        }

        FetchHandler = (func: (marshaledItem: TModel, container: TModel[], itemsCount?: number, allQueryCount?: number) => void) => {
            this._fetchhandler = func
            return this
        }

        DefaultPagination = (itemsForPage: number) => {
            this._pagination = itemsForPage || null
            return this
        }

        CurStartNum = () => (this.Skip || 0) + 1
        CurEndNum = () => {
            let up = this.Skip + this.Take
            return up > this.MaxQuery ? this.MaxQuery : up
        }

        Any = () => {
            return this.Items.length > 0
        }

        protected _pagination = null
        protected _marshaller: (item: TModel) => TModel = (x) => x

        protected _fetchhandler: (marshaledItem: TModel, container: TModel[], itemsCount?: number, allQueryCount?: number) => void =
        (i, cont) => cont.push(i)

        protected _settings: col.IDictionary<GridSetting>
        protected Take: number = null
        protected Skip: number = null
        protected MaxQuery: number = 0
        protected Items: TModel[] = []
        /**raw object to referencing from angular template.
        */
        private Settings: any = {}
    }
}