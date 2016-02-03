/// <reference path="../typings/tsd.d.ts" />
import { ICommandEmitter, ICommandResponseListener, ICommandRequestSender, ICommandResponse } from "./interfaces";

export class SignalRListener implements ICommandResponseListener {
    private _hubConnection: HubConnection;
    private _receivedResponses: Array<ICommandResponse>;
    
    constructor(connection: HubConnection, hubName: string) {
        this._receivedResponses = new Array<ICommandResponse>();
        this._hubConnection = connection;
        this.callbackId = this._hubConnection.hub.id;
        this._hubConnection.proxies[hubName].client.ReceiveCommandResponse = data => {
            this._receivedResponses.push(data);
        };
    }

    callbackId: string 

    listen(commandId: string, callbackId: string): Promise<ICommandResponse>{
        if (callbackId !== this._hubConnection.id)
            throw new Error('Try to listen to the wrong ConnectionId...');
            
        return new Promise<ICommandResponse>(function(resolve, reject){
             var interval = setInterval(() => {
                this._receivedResponses.forEach((r, idx, ar) => {
                    if (r.commandId === commandId) {
                        clearInterval(interval);
                        ar.splice(idx, 1);
                        resolve(r);
                    }
                });
            }, 200);
        });
    }
}