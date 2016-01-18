/// <reference path="../typings/tsd.d.ts" />

import { ICommandEmitter,ICommandResponseListener, ICommandRequestSender, ICommandResponse } from "./interfaces";
import { Command }  from "./Command";

export class CommandEmitter implements ICommandEmitter {
    private _sender: ICommandRequestSender
    private _listener: ICommandResponseListener
    private _prefix: string
    
    constructor(prefix: string, commandRequestSender: ICommandRequestSender);
    constructor(prefix: string, commandRequestSender: ICommandRequestSender, commandResponseListener?: ICommandResponseListener) {
        this._prefix = prefix;
        this._sender = commandRequestSender;
        this._listener = commandResponseListener;
    }

    public emit(command: Command) {
        console.info('Sending Command : ' + command.name );

        var url = this._prefix + '/' + command.name + '?c=' + (this._listener ? this._listener.callbackId : '');
        var xhr = this._sender.send(url, command);
        
        return xhr.then((data: ICommandResponse) => {

            if (data !== null) {
                switch (data.responseType) {
                    case -1: throw new Error(data.payload);
                    
                    // Direct resposne
                    case 0: return data;
                    
                    // Deferred response
                    case 1: {
                        if (this._listener == null) {
                            throw new Error("Deferred command execution is not supported by the Server. It should not answer a deferred response type.");
                        }

                        var callbackId: string = data.payload;
                        return this._listener.listen(data.commandId, callbackId);
                    }
                }
            }
        });
    }
}