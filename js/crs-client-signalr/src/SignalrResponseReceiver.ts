import { ResponseReceiver, CommandResponse, ResponseType, EndpointCommandMetadata } from "@signature/crs-client";
import { LogLevel } from '@microsoft/signalr';
import { HubConnectionManager } from "./HubConnectionManager";
import { ResponseProxy } from "./ResponseProxy";

export class SignalrResponseReceiver implements ResponseReceiver {
    private readonly hubConnectionManager: HubConnectionManager;
    private readonly responseProxyMap: Map<string,ResponseProxy>;
    private _callerId: string|undefined;
    private readonly _callerIdCallbacks: ((callerId: string, error: any) => void)[];

    public constructor(signalrUrl: string, logLevel?: LogLevel) {
        if(logLevel === undefined) {
            logLevel = LogLevel.Error;
        }
        this.hubConnectionManager = new HubConnectionManager(signalrUrl, logLevel);
        this.responseProxyMap = new Map();
        this._callerIdCallbacks = [];
    }

    public processCommandResponse<TResult>(r: CommandResponse<TResult>): Promise<CommandResponse<TResult>> {
        if(r.responseType === ResponseType.Asynchronous) {
            console.debug(`CRS-SignalR: Waiting for deferred response - CommandId: ${r.commandId}`);
            return new Promise( (resolve, reject) => {
                this.ensureResponseProxy(r.commandId).setResolver(
                    // Resolve
                    (responseType: ResponseType, result: TResult) => {
                        console.debug(`CRS-SignalR: Processing deferred command response - CommandId: ${r.commandId}`);
                        this.clearResponseProxy(r.commandId);
                        resolve({
                            commandId: r.commandId,
                            responseType: responseType,
                            payload: result
                        });
                    },
                    // Reject
                    (reason: any) => {
                        console.debug(`CRS-SignalR: Caught error - CommandId: ${r.commandId}`);
                        this.clearResponseProxy(r.commandId);
                        reject(reason);
                    }
                );
            });
        }
        return Promise.resolve(r);
    }

    public getCallerId(commandName: string, commandMetadata: EndpointCommandMetadata|undefined): Promise<string|undefined> {
        if(commandMetadata === undefined) {
            console.warn(`CRS-SignalR: No metadata is available for command ${commandName}. The SignalR CallerId will be used.`);
            return this.internalGetCallerId();
        }
        const commandTraits = commandMetadata.traits ? commandMetadata.traits.split('|') : [];
        if(commandTraits.indexOf('FireForget') >= 0) {
            return this.internalGetCallerId();
        }

        return Promise.resolve(undefined);
    }

    private internalGetCallerId(): Promise<string> {
        if(this._callerId === undefined) {
            return new Promise<string>((resolve, reject) => {
                const callback: (callerId: string, error: any) => void = (callerId, error) => {
                    if(error) {
                        reject(error)
                    } else {
                        resolve(callerId);
                    }
                }
                this._callerIdCallbacks.push(callback);
            });
        } else {
            return Promise.resolve(this._callerId);
        }
    }

    public initialize(): Promise<void> {
        this._callerId = undefined;
        this.hubConnectionManager.onClosed(() => this.onDisconnect());
        this.hubConnectionManager.on('ReceiveCallerId', (callerId) => {
            console.debug(`CRS-SignalR: Received CallerId: ${callerId}`);
            this._callerId = callerId;
            for(let i = 0; i < this._callerIdCallbacks.length; i++) {
                this._callerIdCallbacks[i](callerId, undefined);
            }
            // Clear array in-place
            this._callerIdCallbacks.splice(0, this._callerIdCallbacks.length);
        });
        this.hubConnectionManager.on('ReceiveResult', (sourceCommandId, responseType, response) => {
            this.onResult(sourceCommandId, responseType, response);
        } );

        return new Promise( (resolve, reject) => {
            this.hubConnectionManager.connect().then(() => {
                console.debug('CRS-SignalR: Waiting for CallerId');
                this.internalGetCallerId().then(() => {
                    resolve();
                }).catch((e) => reject(e));
            }).catch((e) => reject(e));
        } );
    }

    private ensureResponseProxy(commandId: string): ResponseProxy {
        let proxy = this.responseProxyMap.get(commandId);
        if(!proxy) {
            proxy = new ResponseProxy(commandId);
            this.responseProxyMap.set(commandId, proxy);
        }
        return proxy;
    }

    private clearResponseProxy(commandId: string) {
        this.responseProxyMap.delete(commandId);
    }

    private onResult(sourceCommandId: string, responseType: ResponseType, result: any) {
        console.debug(`CRS-SignalR: Received deferred response - CommandId: ${sourceCommandId}`);
        this.ensureResponseProxy(sourceCommandId).setResponse(responseType, result);
    }

    private onDisconnect() {
        console.debug(`CRS-SignalR: Disconnected - Failing all pending commands`);

        this.responseProxyMap.forEach( (proxy) => {
            proxy.setError("CRS SignalR Hub was disconnected");
        });
        this.responseProxyMap.clear();

        for(let i = 0; i < this._callerIdCallbacks.length; i++) {
            this._callerIdCallbacks[i]('', "CRS SignalR Hub was disconnected");
        }
        // Clear array in-place
        this._callerIdCallbacks.splice(0, this._callerIdCallbacks.length);
        this._callerId = undefined;
    }
}
