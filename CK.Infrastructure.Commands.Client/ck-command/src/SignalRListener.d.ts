/// <reference path="../typings/tsd.d.ts" />
import { ICommandResponseListener, ICommandResponse } from "./interfaces";
export declare class SignalRListener implements ICommandResponseListener {
    private _hubConnection;
    private _receivedResponses;
    constructor(connection: HubConnection, hubName: string);
    callbackId: string;
    listen(commandId: string, callbackId: string): Promise<ICommandResponse>;
}
