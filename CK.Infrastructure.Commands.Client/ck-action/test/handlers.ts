import * as Ck from '../core';

@Ck.ActionHandler("test")
export class TestHandler implements Ck.IActionHandler{
    handle(action: Ck.Action) : Promise<any>{
        return Promise.resolve(action.properties.a + action.properties.b);
    }
}

export class WrongHandler implements Ck.IActionHandler{
    handle(action: Ck.Action) : Promise<any>{
        return Promise.resolve(action.properties.a + action.properties.b);
    }
}

@Ck.ActionHandler("test")
export class DuplicateTestHandler implements Ck.IActionHandler{
    handle(action: Ck.Action) : Promise<any>{
        return Promise.resolve(action.properties.a + action.properties.b);
    }
}

@Ck.ActionHandler("test")
export class WrongTestHandler {
    doStuff(){
        
    }
}
export class DumbActivator implements Ck.Activator{
    activate<T>(type: Function) {
        if(type == TestHandler){
            return new TestHandler();
        }
    }
}