/// <reference path="../typings/tsd.d.ts" />
import { ICommandEmitter, ICommandRequestSender, ICommandResponse } from "./interfaces";
import { Command } from "./Command";
export declare class CommandEmitter implements ICommandEmitter {
    private _sender;
    private _listener;
    private _prefix;
    constructor(prefix: string, commandRequestSender: ICommandRequestSender);
    emit(command: Command): Promise<ICommandResponse>;
}
