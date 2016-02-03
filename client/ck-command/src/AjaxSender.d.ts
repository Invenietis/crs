/// <reference path="../typings/tsd.d.ts" />
import { ICommandRequestSender } from "./Interfaces";
import { Command } from "./Command";
export declare class AjaxSender implements ICommandRequestSender {
    send(url: string, command: Command): Promise<any>;
}
