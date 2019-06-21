import { ResponseType } from "@signature/crs-client";
export class ResponseProxy {
    private responseType?: ResponseType;
    private result?: any;
    private error?: any;
    private resolve?: Function;
    private reject?: Function;
    constructor(private readonly commandId: string) {
    }
    public setResponse(responseType: ResponseType, result: any) {
        this.responseType = responseType;
        this.result = result;
        this.checkResolve();
    }
    public setError(reason: any) {
        this.error = reason;
        this.checkResolve();
    }
    public setResolver(resolve: Function, reject: Function) {
        this.resolve = resolve;
        this.reject = reject;
        this.checkResolve();
    }
    private checkResolve() {
        if (this.resolve && this.responseType) {
            this.resolve(this.responseType, this.result);
        }
        else if (this.reject && this.error) {
            this.reject(this.error);
        }
    }
}
