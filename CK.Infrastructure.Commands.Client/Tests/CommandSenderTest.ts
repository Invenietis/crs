/// <reference path="../Scripts/jquery.d.ts"/>
import tsUnit = require('../Scripts/tsUnit');
import Commands = require('../CommandSender'); 

export class CommandSenderTests extends tsUnit.TestClass {

    private target = new Commands.CommandSender({
        post(url: string, command) {

            var deferred = $.Deferred<any>();
            var promise = deferred.promise();

            // Simulate async request
            setTimeout(() => {
                deferred.resolve(JSON.stringify( { 
                    CommandId: '1234', 
                    Payload: 'connectionId',
                    ResponseType: 1 // Deferred
                })); 
            }, 1000);
            return promise; 
        }
    }, {
        listen: (commandId, callbackId): JQueryPromise<Commands.ICommandResponse> => {

            var deferred = $.Deferred<Commands.ICommandResponse>();
            var promise = deferred.promise();
            // Simulate async request
            setTimeout(() => {
                var response = {
                    Payload: {
                        EffectiveDate: new Date()
                    },
                    CommandId: '1234',
                    ResponseType: 0
                };
                deferred.resolve(response);
            }, 1000);
            return promise;
        }
    });

    sendACommandShouldTriggerAnXhrRequestToTheServer() {
        var command = {
            sourceAccountId: '7A8125D3-2BF9-45DE-A258-CE0D3C17892D',
            destinationAccountId: '37EC9EA1-2A13-4A4D-B55E-6C844D822DAC',
            amount: '500'
        };
        var me = this;
        var promise = this.target.send('TransferAmount', command);
        promise.done(response => {
            me.isTrue(response != null);
            me.isTrue(response.Payload != null);
            me.isTrue(response.Payload.EffectiveDate != null);
            console.log(response);
        });
    }
}