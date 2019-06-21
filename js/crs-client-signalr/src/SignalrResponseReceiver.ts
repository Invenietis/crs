import { ResponseReceiver, CommandResponse, ResponseType } from "@signature/crs-client";
import { LogLevel } from '@aspnet/signalr';
import { HubConnectionManager } from "./HubConnectionManager";
import { ResponseProxy } from "./ResponseProxy";

export class SignalrResponseReceiver implements ResponseReceiver {
    private readonly hubConnectionManager: HubConnectionManager;
    private readonly repsonseProxyMap: Map<string,ResponseProxy>;
    private callerId: string = '';

    public constructor(signalrUrl: string, logLevel?: LogLevel) {
        if(logLevel === undefined) {
            logLevel = LogLevel.Error;
        }
        this.hubConnectionManager = new HubConnectionManager(signalrUrl, logLevel);
        this.repsonseProxyMap = new Map();
    }

    public processCommandResponse<TResult>(r: CommandResponse<TResult>): Promise<CommandResponse<TResult>> {
        if(r.responseType === ResponseType.Asynchronous) {
            console.debug(`CRS: Waiting for deferred response - CommandId: ${r.commandId}`);
            return new Promise( (resolve, reject) => {
                this.ensureResponseProxy(r.commandId).setResolver(
                    // Resolve
                    (responseType: ResponseType, result: TResult) => {
                        console.debug(`CRS: Processing deferred command response - CommandId: ${r.commandId}`);
                        this.clearResponseProxy(r.commandId);
                        resolve({
                            commandId: r.commandId,
                            responseType: responseType,
                            payload: result
                        });
                    },
                    // Reject
                    (reason: any) => {
                        console.debug(`CRS: Caught error - CommandId: ${r.commandId}`);
                        this.clearResponseProxy(r.commandId);
                        reject(reason);
                    }
                );
            });
        }
        return Promise.resolve(r);
    }

    public getCallerId(): string {
        return this.callerId;
    }

    public async initialize(): Promise<void> {
        this.hubConnectionManager.onClosed(() => this.onDisconnect());
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
        let proxy = this.repsonseProxyMap.get(commandId);
        if(!proxy) {
            proxy = new ResponseProxy(commandId);
            this.repsonseProxyMap.set(commandId, proxy);
        }
        return proxy;
    }

    private clearResponseProxy(commandId: string) {
        this.repsonseProxyMap.delete(commandId);
    }

    private onResult(sourceCommandId: string, responseType: ResponseType, result: any) {
        console.debug(`CRS: Received deferred response - CommandId: ${sourceCommandId}`);
        this.ensureResponseProxy(sourceCommandId).setResponse(responseType, result);
    }

    private onDisconnect() {
        console.debug(`CRS: Disconnected - Failing all pending commands`);
        this.repsonseProxyMap.forEach( (proxy) => {
            proxy.setError("CRS SignalR Hub was disconnected");
        });
        this.repsonseProxyMap.clear();
    }
}
