var TranslatonUtils;
(function (TranslatonUtils) {
    var col = TSNetLike.Collections;
    var Translator = (function () {
        function Translator(rawDictionary) {
            var _this = this;
            this.Translate = function (Str) {
                var Args = [];
                for (var _i = 1; _i < arguments.length; _i++) {
                    Args[_i - 1] = arguments[_i];
                }
                var val = _this.dict.cont(Str) ? _this.dict.take(Str) : Str;
                if (typeof Args === 'undefined' || Args === null)
                    return val;
                return StringExt.Format(val, Args);
            };
            this.dict = new col.Dictionary(rawDictionary);
        }
        return Translator;
    }());
    TranslatonUtils.Translator = Translator;
})(TranslatonUtils || (TranslatonUtils = {}));
