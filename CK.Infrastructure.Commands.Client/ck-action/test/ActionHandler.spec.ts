/// <reference path="../typings/tsd.d.ts" />
import * as Ck from '../core'
import * as test from './handlers'
import {Command} from 'ck-command/command';

describe("Action Handler Test", function(){
    var activator: Ck.Activator = {
        activate: function<T>(type: Function) {
            if(type == test.TestHandler){
                return new test.TestHandler();
            }
        }
    }; 
    
    var resolver = new Ck.CommandResolver(activator);
    resolver.registerHandler(test.TestHandler);
    
    it("Handler should be executed", function(done){
        
        var sender = new Ck.ActionSender(resolver);
        
        sender.send(new Command("test", {
            a: 1,
            b: 6
        })).then( r => {
            expect(r).toBe(7);
            done();
        });
    });
    
    it("ActionSender should not found the handler and throw an exception", function(){
        var sender = new Ck.ActionSender(resolver);
        
        return expect(function(){
            sender.send(new Command("tests", {
                a: 1,
                b: 6
            }));
        }).toThrow("No handler found for the command tests");
    });
    
});