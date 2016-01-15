/// <reference path="../typings/tsd.d.ts" />
declare module "src/Command" {
    export class Command {
        name: string;
        properties: any;
        constructor(name: string, properties: {});
    }
}
declare module "src/Interfaces" {
    import { Command } from "src/Command";
    export interface ICommandResponse {
        /**
        * The identifier of the command. */
        commandId: string;
        /**
        * The payload of the result. */
        payload: any;
        responseType: Number;
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
}
declare module "src/AjaxSender" {
    import { ICommandRequestSender } from "src/Interfaces";
    import { Command } from "src/Command";
    export class AjaxSender implements ICommandRequestSender {
        send(url: string, command: Command): Promise<any>;
    }
}
declare module "src/CommandEmitter" {
    import { ICommandEmitter, ICommandResponseListener, ICommandRequestSender, ICommandResponse } from "src/Interfaces";
    import { Command } from "src/Command";
    export class CommandEmitter implements ICommandEmitter {
        private _sender;
        private _listener;
        private _prefix;
        constructor(prefix: string, commandRequestSender: ICommandRequestSender, commandResponseListener?: ICommandResponseListener);
        emit(command: Command): Promise<ICommandResponse>;
    }
}
declare module "src/SignalRListener" {
    import { ICommandResponseListener, ICommandResponse } from "src/Interfaces";
    export class SignalRListener implements ICommandResponseListener {
        private _hubConnection;
        private _receivedResponses;
        constructor(connection: HubConnection, hubName: string);
        callbackId: string;
        listen(commandId: string, callbackId: string): Promise<ICommandResponse>;
    }
}
declare module "command" {
    export * from "src/Command";
    export * from "src/AjaxSender";
    export * from "src/CommandEmitter";
    export * from "src/Interfaces";
    export * from "src/SignalRListener";
}
