/// <reference path="../typings/tsd.d.ts" />

/**
 * Action executor decorator. This must be used on every executor class
 * @param metadata { actionName The name of the targeted action }
 */
export function ActionExecutor<TFunction extends Function>( metadata: { actionName:string } ): Function {
    return function(target: Function){
        (<any>target).__meta = metadata;
        return target
    };
}

/**
 * All ActionExector need to satisify the IActionExecutor interface in order to be excecuted
 */
export interface IActionExecutor {
    execute(parameters: any) : Promise<any>;
}