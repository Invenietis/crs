/// <reference path="../typings/tsd.d.ts" />
import {CommandEmitter} from '../src/CommandEmitter';
import {Command} from '../src/Command';
import {ICommandResponse} from '../src/Interfaces';

describe("Command Emitter Send Tests", function() {
    var date = new Date();
    var commandSender = new CommandEmitter('/c', {
        send(url: string, command: Command) {
            return new Promise<any>(function(resolve, reject){
                // Simulate async request
                setTimeout(() => { 
                    resolve({
                        commandId: '1234',
                        payload: '3712', 
                        responseType: 1 // Deferred 
                    });
                }, 1000);
            });
        }
    }, {
        callbackId: '3712',
        listen: (commandId, callbackId): Promise<ICommandResponse> => {
            return new Promise<any>(function(resolve, reject){
                // Simulate async request
                setTimeout(() => { 
                    var response = {
                        payload: {
                            effectiveDate: date
                        },
                        commandId: '1234',
                        responseType: 0
                    };
                    resolve(response);
                }, 1000); 
            });
        }
    });
    
    it("Send a command should trigger an Xhr request to the server", function(done){
         var command = new Command ('TransferAmount', {
            sourceAccountId: '7A8125D3-2BF9-45DE-A258-CE0D3C17892D',
            destinationAccountId: '37EC9EA1-2A13-4A4D-B55E-6C844D822DAC',
            amount: '500'
        });
        
        var promise = commandSender.emit(command);
        promise.then(response => {
            expect(response).toBeDefined(); 
            expect(response.payload).toBeDefined();
            expect(response.payload.effectiveDate).toBe(date);
            expect(response.commandId).toBe('1234');
            expect(response.responseType).toBe(0);
            done();
        });
    });
});