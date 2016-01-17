/// <reference path="../typings/tsd.d.ts" />
import * as Ck from '../core'
import * as handlers from './handlers'

describe("ActionResolver Tests", function(){
   it("Resolver should not accept a handler without the ActionHandler decorator", function(){
        var resolver = new Ck.ActionResolver(new handlers.DumbActivator());
        
         return expect(function(){
           resolver.registerHandler(handlers.WrongHandler);
        }).toThrow("The handler WrongHandler has no associated action. Please use the ActionHandler decorator to specify one");
    });
    
    it("Resolver should not accept a handler that not satisfy the IActionHandler interface", function(){
        var resolver = new Ck.ActionResolver(new handlers.DumbActivator());
        
        return expect(function(){
            resolver.registerHandler(handlers.WrongTestHandler);
        }).toThrow("The handler WrongTestHandler does not satisfy the IActionHandler interface");
    });
    
    it("Resolver should not allow the registration of a handler more than once", function(){
        var resolver = new Ck.ActionResolver(new handlers.DumbActivator());
        resolver.registerHandler(handlers.TestHandler);
        
        return expect(function(){
           resolver.registerHandler(handlers.TestHandler);
        }).toThrow("The handler TestHandler is already registered");
    });
    
    it("Resolver should not allow the registration of differents handlers for the same action", function(){
        var resolver = new Ck.ActionResolver(new handlers.DumbActivator());
        resolver.registerHandler(handlers.TestHandler);
        
        return expect(function(){
           resolver.registerHandler(handlers.DuplicateTestHandler);
        }).toThrow("Cannot register DuplicateTestHandler: The handler TestHandler is already registered for the command test");
    });
    
    it("Resolver should resolve the previously registered handler", function(){
        var resolver = new Ck.ActionResolver(new handlers.DumbActivator());
        resolver.registerHandler(handlers.TestHandler);
        var handler = resolver.resolve('test');
        
        return expect(handler instanceof handlers.TestHandler).toBeTruthy();
    });
    
    it("Resolver should not found the handler", function(){
        var resolver = new Ck.ActionResolver(new handlers.DumbActivator());
        return expect(function(){
            resolver.resolve('test');
        }).toThrow("No handler found for the action test");
    });
});