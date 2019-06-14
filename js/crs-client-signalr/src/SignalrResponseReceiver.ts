import { ResponseReceiver, CommandResponse, ResponseType } from "@signature/crs-client";
import { LogLevel } from '@aspnet/signalr';
import { HubConnectionManager } from "./HubConnectionManager";

class ResponseProxy {
    private responseType?: ResponseType;
    private result?: any;
    private resolver?: Function;

    constructor(
        private readonly commandId: string
        ) {
    }

    public setResponse(responseType: ResponseType, result: any) {
        this.responseType = responseType;
        this.result = result;
        this.checkResolve();
    }

    public setResolver(resolver: Function) {
        this.resolver = resolver;
        this.checkResolve();
    }

    private checkResolve() {
        if(this.responseType && this.resolver) {
            this.resolver(this.responseType, this.result);
        }
    }
}

export class SignalrResponseReceiver implements ResponseReceiver {
    private readonly hubConnectionManager: HubConnectionManager;
    private readonly map: Map<string,ResponseProxy>;
    private callerId: string = '';
    
    public constructor(signalrUrl: string, logLevel?: LogLevel) {
        if(logLevel === undefined) {
            logLevel = LogLevel.Error;
        }
        this.hubConnectionManager = new HubConnectionManager(signalrUrl, logLevel);
        this.map = new Map();
    }

    public processCommandResponse<TResult>(r: CommandResponse<TResult>): Promise<CommandResponse<TResult>> {
        if(r.responseType === ResponseType.Asynchronous) {
            console.debug(`CRS: Waiting for deferred response - CommandId: ${r.commandId}`);
            return new Promise( (resolve, reject) => {
                this.ensureResponseProxy(r.commandId).setResolver((responseType: ResponseType, result: TResult) => {
                    console.debug(`CRS: Processing deferred command response - CommandId: ${r.commandId}`);
                    this.clearResponseProxy(r.commandId);
                    resolve({
                        commandId: r.commandId,
                        responseType: responseType,
                        payload: result
                    });
                });
            });
        }
        return Promise.resolve(r);
    }

    public getCallerId(): string {
        return this.callerId;
    }

    public async initialize(): Promise<void> {
        await this.hubConnectionManager.connect().then((hubConnection) => {
            hubConnection.on('ReceiveCallerId', (connectionId) => {
                this.callerId = connectionId;
            });
            hubConnection.on('ReceiveResult', (sourceCommandId, responseType, response) => {
                this.onResult(sourceCommandId, responseType, response);
            } );
        });
    }

    private ensureResponseProxy(commandId: string): ResponseProxy {
        let proxy = this.map.get(commandId);
        if(!proxy) {
            proxy = new ResponseProxy(commandId);
            this.map.set(commandId, proxy);
        }
        return proxy;
    }

    private clearResponseProxy(commandId: string) {
        this.map.delete(commandId);
    }

    private onResult(sourceCommandId: string, responseType: ResponseType, result: any) {
        console.debug(`CRS: Received deferred response - CommandId: ${sourceCommandId}`);
        this.ensureResponseProxy(sourceCommandId).setResponse(responseType, result);
    }
}