import {Command} from 'ck-command/command';
import * as Ck from '../core';

@Ck.ActionHandler("test")
export class TestHandler implements Ck.IActionHandler{
    handle(action: Command) : Promise<any>{
        
        console.log(action);
        return Promise.resolve(action.properties.a + action.properties.b);
    }
}