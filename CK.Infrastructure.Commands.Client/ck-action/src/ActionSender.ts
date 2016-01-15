/// <reference path="../typings/tsd.d.ts" />
import { Command } from 'ck-command/command';
import { CommandResolver } from './Resolver';

export class ActionSender {
    constructor(private _resolver: CommandResolver){
    }
    
    send (command: Command) : Promise<any>{
        var handler = this._resolver.resolve( command );
            
        return handler.handle(command);
    }
}