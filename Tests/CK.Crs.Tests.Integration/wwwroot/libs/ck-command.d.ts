/// <reference path="../typings/tsd.d.ts" />

declare namespace CK {
    export class Command {
    
        name: string;
        properties: any;
    
        constructor(name: string, properties: {});
    }
    
    export interface CommandResponse {
        /**
        * The identifier of the command. */
        commandId: string,
        /**
        * The payload of the result. */
        payload: any,

        responseType: Number
    }

    export interface ICommandEmitter {
        
        emit: (command: Command) => Promise<CommandResponse>;
    }

    export interface CommandResponseListener {
        
        listen: (commandId: string, callbackId: string) => Promise<CommandResponse>;
        
        callbackId: string;
    }

    export interface CommandRequestSender {
        send: (endpoint: string, command: Command) => Promise<any>;
    }
    
    export class AjaxSender implements CommandRequestSender {
        send(url: string, command: Command): Promise<any>;
    }
    
    export class CommandEmitter implements ICommandEmitter {
        private _sender;
        private _listener;
        private _prefix;
        constructor(prefix: string, commandRequestSender: CommandRequestSender, commandResponseListener?: CommandResponseListener);
        emit(command: Command): Promise<CommandResponse>;
    }
    
    export class SignalRListener implements CommandResponseListener {
        private _hubConnection;
        private _receivedResponses;
        constructor(connection: HubConnection, hubName: string);
        callbackId: string;
        listen(commandId: string, callbackId: string): Promise<CommandResponse>;
    }
}