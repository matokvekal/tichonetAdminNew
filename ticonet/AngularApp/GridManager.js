var AngularApp;
(function (AngularApp) {
    var col = TSNetLike.Collections;
    var GridSetting = (function () {
        function GridSetting() {
        }
        return GridSetting;
    }());
    AngularApp.GridSetting = GridSetting;
    var GridManager = (function () {
        function GridManager(Fetcher, fetchUrlAlias) {
            var _this = this;
            this.Fetcher = Fetcher;
            this.fetchUrlAlias = fetchUrlAlias;
            this.Clear = function () { return _this.Items = []; };
            this.IsNextPage = function () { return _this.Take !== null || _this.MaxQuery > _this.Skip + _this.Take; };
            this.IsPrevPage = function () { return _this.Skip !== null || _this.Skip !== 0; };
            this.Next = function (skip, take) {
                _this.Skip = (_this.Skip || 0) + (skip || _this._pagination);
                _this.Take = (take || _this._pagination);
                return _this;
            };
            this.Prev = function (skip, take) {
                _this.Skip = (_this.Skip || 0) - (skip || _this._pagination);
                _this.Take = (take || _this._pagination);
                if (_this.Skip < 0)
                    _this.Skip = 0;
                return _this;
            };
            this.Reset = function (take) {
                _this.Skip = 0;
                _this.Take = take || _this._pagination;
                return _this;
            };
            this.ResetSettings = function () {
                _this._settings.iter(function (k, v) {
                    _this.Settings[k] = v.Default;
                });
                return _this;
            };
            this.Refetch = function (background, onSucces, onFailed) {
                var args = _this.BuildRequestArgs(onSucces, onFailed);
                _this.Fetcher.Request(!background, args);
            };
            this.RefetchDelayed = function (delayMilisecs, background, onSucces, onFailed) {
                _this.Refetch(background, onSucces, onFailed);
            };
            this.BuildRequestArgs = function (onSucces, onFailed) {
                //Use this if you wrap onSucces/onFail here:
                //HandleCallBacks(onSucces, onFailed)
                //then after AJAX:
                //RunCallbackOrHandler(onSucces)
                //RunCallbackOrHandler(onFailed)
                AngularApp.HandleCallBacks(onSucces);
                var cb = function (r) {
                    _this.MaxQuery = r.data.allquerycount;
                    _this.Items.splice(0, _this.Items.length);
                    r.data.items.forEach(function (x) { return _this.Items.push(_this._marshaller(x)); });
                    AngularApp.RunCallbackOrHandler(onSucces, r);
                };
                var params = new AngularApp.FetchParams()
                    .addSkip(_this.Skip)
                    .addCount(_this.Take);
                _this._settings.iter(function (k, v) {
                    var val = _this.Settings[k];
                    if (v.Predicate(val, _this.Settings))
                        v.ParamMaker(params, val);
                });
                var args = {
                    urlalias: _this.fetchUrlAlias,
                    onSucces: cb,
                    onFailed: onFailed,
                    params: params
                };
                return args;
            };
            /**
            If no predicate passed, the predicate
            '(x) => typeof x !== "undefined"'
            will be used
            */
            this.AddSetting = function (Name, Default, ParamMaker, Predicate) {
                _this.Settings[Name] = Default;
                _this._settings.add(Name, {
                    Name: Name,
                    Default: Default,
                    ParamMaker: ParamMaker,
                    Predicate: Predicate || (function (x) { return typeof x !== 'undefined'; })
                });
                return _this;
            };
            this.SetSettingValue = function (Name, Value) {
                _this[Name] = Value;
                return _this;
            };
            this.Marshaller = function (func) {
                _this._marshaller = func;
                return _this;
            };
            this.DefaultPagination = function (itemsForPage) {
                _this._pagination = itemsForPage || null;
                return _this;
            };
            this.CurStartNum = function () { return (_this.Skip || 0) + 1; };
            this.CurEndNum = function () {
                var up = _this.Skip + _this.Take;
                return up > _this.MaxQuery ? _this.MaxQuery : up;
            };
            this.Any = function () {
                return _this.Items.length > 0;
            };
            this._pagination = null;
            this._marshaller = function (x) { return x; };
            this.Take = null;
            this.Skip = null;
            this.MaxQuery = 0;
            this.Items = [];
            /**raw object to referencing from angular template.
            */
            this.Settings = {};
            this._settings = new col.Dictionary();
        }
        return GridManager;
    }());
    AngularApp.GridManager = GridManager;
})(AngularApp || (AngularApp = {}));
