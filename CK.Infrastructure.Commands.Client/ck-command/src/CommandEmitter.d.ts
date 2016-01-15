/// <reference path="../typings/tsd.d.ts" />
import { ICommandEmitter, ICommandResponseListener, ICommandRequestSender, ICommandResponse } from "./interfaces";
import { Command } from "./Command";
export declare class CommandEmitter implements ICommandEmitter {
    private _sender;
    private _listener;
    private _prefix;
    constructor(prefix: string, commandRequestSender: ICommandRequestSender, commandResponseListener?: ICommandResponseListener);
    emit(command: Command): Promise<ICommandResponse>;
}
