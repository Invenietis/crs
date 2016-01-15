/// <reference path="../typings/tsd.d.ts" />
import { ICommandEmitter, Command, ICommandResponse } from 'ck-command/command';
import {Action} from './Action';

export class ActionEmitter extends ICommandEmitter{
    
    emit (action: Command) : Promise<ICommandResponse>{
        return new Promise<ICommandResponse>(function(resolve, reject){
            
        });
    }
}