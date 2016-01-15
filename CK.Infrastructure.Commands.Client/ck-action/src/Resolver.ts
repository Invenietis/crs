import { Activator } from './Activator';
import { IActionHandler } from './ActionHandler';

import { Command } from 'ck-command/command';

export class CommandResolver{
    private _handlers : { [commanName: string]: { type: Function, instance: IActionHandler } } = {};
   
    constructor(private _activator: Activator){}
    
    registerHandler( handler: Function ){
        var h = <any>handler;
        if(!h.__cmd){
            throw `The handler ${handler} has no associated command. Please use the ActionHandler decorator to specify one`;
        }
        
        this._handlers[h.__cmd] = { 
            type: handler, 
            instance: undefined 
        };
    }
    
    resolve(command: Command): IActionHandler {
        var handlerInfo = this._handlers[command.name];
        if(handlerInfo == undefined) throw `No handler found for the command ${command.name}`;
        
        //create and store the handler instance
        if(handlerInfo.instance == undefined){
            handlerInfo.instance = this._activator.activate<IActionHandler>(handlerInfo.type);
        }
        
        return handlerInfo.instance;
    }
}