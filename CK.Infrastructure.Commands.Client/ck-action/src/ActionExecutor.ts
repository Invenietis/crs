/// <reference path="../typings/tsd.d.ts" />

export function ActionExecutor<TFunction extends Function>( metadata: { actionName:string } ): Function {
    return function(target: Function){
        (<any>target).__meta = metadata;
        return target
    };
}

export interface IActionExecutor {
    execute(parameters: any) : Promise<any>;
}