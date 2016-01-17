/// <reference path="../typings/tsd.d.ts" />
import { Action } from './Action';
import { ActionResolver } from './Resolver';

export class ActionSender {
    constructor(private _resolver: ActionResolver){
    }
    
    send (action: Action) : Promise<any>{
        var handler = this._resolver.resolve( action.name );
            
        return handler.handle(action);
    }
}