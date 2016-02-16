/// <reference path="../typings/tsd.d.ts" />

declare namespace CK{
    export class Command {
    
        name: string;
        properties: any;
    
        constructor(name: string, properties: {});
    }
    
    export interface ICommandResponse {
        /**
        * The identifier of the command. */
        commandId: string,
        /**
        * The payload of the result. */
        payload: any,

        responseType: Number
    }

    export interface ICommandEmitter {
        
        emit: (command: Command) => Promise<ICommandResponse>;
    }

    export interface ICommandResponseListener {
        
        listen: (commandId: string, callbackId: string) => Promise<ICommandResponse>;
        
        callbackId: string;
    }

    export interface ICommandRequestSender {
        send: (endpoint: string, command: Command) => Promise<any>;
    }
    
    export class AjaxSender implements ICommandRequestSender {
        send(url: string, command: Command): Promise<any>;
    }
    
    export class CommandEmitter implements ICommandEmitter {
        private _sender;
        private _listener;
        private _prefix;
        constructor(prefix: string, commandRequestSender: ICommandRequestSender, commandResponseListener?: ICommandResponseListener);
        emit(command: Command): Promise<ICommandResponse>;
    }
    
    export class SignalRListener implements ICommandResponseListener {
        private _hubConnection;
        private _receivedResponses;
        constructor(connection: HubConnection, hubName: string);
        callbackId: string;
        listen(commandId: string, callbackId: string): Promise<ICommandResponse>;
    }
}