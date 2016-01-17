import { Activator } from './Activator';
import { IActionHandler } from './ActionHandler';

import { Action } from './Action';

export class ActionResolver{
    private _handlers : { [actionName: string]: { type: Function, instance: IActionHandler } } = {};
   
    constructor(private _activator: Activator){}
    
    registerHandler( handler: Function ){
        var h = <any>handler;
        if(!h.__cmd){
            throw `The handler ${handler.name} has no associated action. Please use the ActionHandler decorator to specify one`;
        }
        if(typeof handler.prototype.handle != 'function'){
            throw `The handler ${handler.name} does not satisfy the IActionHandler interface`;
        }
        if(this._handlers[h.__cmd] != undefined){
            if(this._handlers[h.__cmd].type == handler){
                throw `The handler ${handler.name} is already registered`
            }
            
            throw `Cannot register ${handler.name}: The handler ${this._handlers[h.__cmd].type.name} is already registered for the command ${h.__cmd}`;
        }
        
        this._handlers[h.__cmd] = { 
            type: handler, 
            instance: undefined 
        };
    }
    
    resolve(actionName: string): IActionHandler {
        var handlerInfo = this._handlers[actionName];
        if(handlerInfo == undefined) throw `No handler found for the action ${actionName}`;
        
        //create and store the handler instance
        if(handlerInfo.instance == undefined){
            handlerInfo.instance = this._activator.activate<IActionHandler>(handlerInfo.type);
        }
        
        return handlerInfo.instance;
    }
}