export class AmbientValuesProvider {
    private _values: any;
    get values() {
        return this._values;
    } 

    constructor(values?: any) {
        this._values = values;
    }
    
    setValues(values: any) {
        this._values = values;
    }

    merge(obj: any): any {
        return {...obj, ...this.values};
    }
}
