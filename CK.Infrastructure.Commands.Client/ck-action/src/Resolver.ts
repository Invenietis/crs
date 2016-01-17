import { Activator } from './Activator';
import { IActionExecutor } from './ActionExecutor';


export class ActionResolver{
    private _executors : { [actionName: string]: { type: Function, instance: IActionExecutor } } = {};
   
    constructor(private _activator: Activator){}
    
    registerExecutor( executor: Function ){
        var ex = <any>executor;
        if(!ex.__meta || !ex.__meta.actionName){
            throw `The executor ${ex.name} has no associated action. Please use the ActionExecutor decorator to specify one`;
        }
        if(typeof executor.prototype.execute != 'function'){
            throw `The executor ${ex.name} does not satisfy the IActionExecutor interface`;
        }
        if(this._executors[ex.__meta.actionName] != undefined){
            if(this._executors[ex.__meta.actionName].type == executor){
                throw `The executor ${ex.name} is already registered`
            }
            
            throw `Cannot register ${ex.name}: The executor ${(<any>this._executors[ex.__meta.actionName].type).name} is already registered for the action ${ex.__meta.actionName}`;
        }
        
        this._executors[ex.__meta.actionName] = { 
            type: executor, 
            instance: undefined 
        };
    }
    
    resolve(actionName: string): IActionExecutor {
        var executorInfo = this._executors[actionName];
        if(executorInfo == undefined) throw `No executor found for the action ${actionName}`;
        
        //create and store the executor instance
        if(executorInfo.instance == undefined){
            executorInfo.instance = this._activator.activate<IActionExecutor>(executorInfo.type);
        }
        
        return executorInfo.instance;
    }
}