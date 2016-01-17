/// <reference path="../typings/tsd.d.ts" />
import * as Ck from '../core'
import * as executors from './executors'

describe("ActionResolver Tests", function(){
   it("Resolver should not accept an executor without the ActionExecutor decorator", function(){
        var resolver = new Ck.ActionResolver(new executors.DumbActivator());
        
         return expect(function(){
           resolver.registerExecutor(executors.WrongExecutor);
        }).toThrow("The executor WrongExecutor has no associated action. Please use the ActionExecutor decorator to specify one");
    });
    
    it("Resolver should not accept an executor that not satisfy the IActionExecutor interface", function(){
        var resolver = new Ck.ActionResolver(new executors.DumbActivator());
        
        return expect(function(){
            resolver.registerExecutor(executors.WrongTestExecutor);
        }).toThrow("The executor WrongTestExecutor does not satisfy the IActionExecutor interface");
    });
    
    it("Resolver should not allow the registration of an executor more than once", function(){
        var resolver = new Ck.ActionResolver(new executors.DumbActivator());
        resolver.registerExecutor(executors.TestExecutor);
        
        return expect(function(){
           resolver.registerExecutor(executors.TestExecutor);
        }).toThrow("The executor TestExecutor is already registered");
    });
    
    it("Resolver should not allow the registration of differents executors for the same action", function(){
        var resolver = new Ck.ActionResolver(new executors.DumbActivator());
        resolver.registerExecutor(executors.TestExecutor);
        
        return expect(function(){
           resolver.registerExecutor(executors.DuplicateTestExecutor);
        }).toThrow("Cannot register DuplicateTestExecutor: The executor TestExecutor is already registered for the action test");
    });
    
    it("Resolver should resolve the previously registered executor", function(){
        var resolver = new Ck.ActionResolver(new executors.DumbActivator());
        resolver.registerExecutor(executors.TestExecutor);
        var executor = resolver.resolve('test');
        
        return expect(executor instanceof executors.TestExecutor).toBeTruthy();
    });
    
    it("Resolver should not found the executor", function(){
        var resolver = new Ck.ActionResolver(new executors.DumbActivator());
        return expect(function(){
            resolver.resolve('test');
        }).toThrow("No executor found for the action test");
    });
});