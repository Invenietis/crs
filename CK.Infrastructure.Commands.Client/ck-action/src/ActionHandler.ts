/// <reference path="../typings/tsd.d.ts" />
import { Command } from 'ck-command/command';

export function ActionHandler<TFunction extends Function>( commandName: string): Function {
    return function(target: Function){
        (<any>target).__cmd = commandName;
        return target
    };
}

export interface IActionHandler {
    handle(action: Command) : Promise<any>;
}