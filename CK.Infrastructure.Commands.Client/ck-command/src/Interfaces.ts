/// <reference path="../typings/tsd.d.ts" />
import { Command } from "./Command";

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