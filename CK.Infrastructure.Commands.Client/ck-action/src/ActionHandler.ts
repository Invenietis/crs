/// <reference path="../typings/tsd.d.ts" />
import { Action } from './Action';

export function ActionHandler<TFunction extends Function>( actionName: string): Function {
    return function(target: Function){
        (<any>target).__cmd = actionName;
        return target
    };
}

export interface IActionHandler {
    handle(action: Action) : Promise<any>;
}