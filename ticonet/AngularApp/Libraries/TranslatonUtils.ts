namespace TranslatonUtils {
    import col = TSNetLike.Collections

    export class Translator {
        private dict: col.IDictionary<string>

        constructor(rawDictionary: any) {
            this.dict = new col.Dictionary<string>(rawDictionary)
        }

        Translate = (Str: string, ...Args: any[]) => {
            let val = this.dict.cont(Str) ? this.dict.take(Str) : Str
            if (typeof Args === 'undefined' || Args === null)
                return val
            return StringExt.Format(val,Args);
        }
    }

}
