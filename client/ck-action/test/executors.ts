import * as Ck from '../core';

@Ck.ActionExecutor({
    actionName:"test"
})
export class TestExecutor implements Ck.IActionExecutor{
    execute(parameters: any) : Promise<any>{
        return Promise.resolve(parameters.a + parameters.b);
    }
}

export class WrongExecutor implements Ck.IActionExecutor{
     execute(parameters: any) : Promise<any>{
        return Promise.resolve(parameters.a + parameters.b);
    }
}

@Ck.ActionExecutor({
    actionName:"test"
})
export class DuplicateTestExecutor implements Ck.IActionExecutor{
     execute(parameters: any): Promise<any>{
        return Promise.resolve(parameters.a + parameters.b);
    }
}

@Ck.ActionExecutor({
    actionName:"test"
})
export class WrongTestExecutor {
    doStuff(){
        
    }
}
export class DumbActivator implements Ck.Activator{
    activate<T>(type: Function) {
        if(type == TestExecutor){
            return new TestExecutor();
        }
    }
}