/// <reference path="../typings/tsd.d.ts" />
import { ActionResolver } from './Resolver';


export class ActionInvoker {
    constructor(private _resolver: ActionResolver){
    }
    
    invoke (actionName: string, parameters:{}) : Promise<any>{
        var handler = this._resolver.resolve( actionName );
            
        return handler.execute(parameters);
    }
}