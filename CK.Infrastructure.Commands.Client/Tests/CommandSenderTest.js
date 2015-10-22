var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
define(["require", "exports", '../Scripts/tsUnit', '../CommandSender'], function (require, exports, tsUnit, Commands) {
    var CommandSenderTests = (function (_super) {
        __extends(CommandSenderTests, _super);
        function CommandSenderTests() {
            _super.apply(this, arguments);
            this.target = new Commands.CommandSender({
                post: function (url, command) {
                    var deferred = $.Deferred();
                    var promise = deferred.promise();
                    // Simulate async request
                    setTimeout(function () {
                        deferred.resolve(JSON.stringify({
                            CommandId: '1234',
                            Payload: 'connectionId',
                            ResponseType: 1 // Deferred
                        }));
                    }, 1000);
                    return promise;
                }
            }, {
                listen: function (commandId, callbackId) {
                    var deferred = $.Deferred();
                    var promise = deferred.promise();
                    // Simulate async request
                    setTimeout(function () {
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
        }
        CommandSenderTests.prototype.sendACommandShouldTriggerAnXhrRequestToTheServer = function () {
            var command = {
                sourceAccountId: '7A8125D3-2BF9-45DE-A258-CE0D3C17892D',
                destinationAccountId: '37EC9EA1-2A13-4A4D-B55E-6C844D822DAC',
                amount: '500'
            };
            var me = this;
            var promise = this.target.send('TransferAmount', command);
            promise.done(function (response) {
                me.isTrue(response != null);
                me.isTrue(response.Payload != null);
                me.isTrue(response.Payload.EffectiveDate != null);
                console.log(response);
            });
        };
        return CommandSenderTests;
    })(tsUnit.TestClass);
    exports.CommandSenderTests = CommandSenderTests;
});
//# sourceMappingURL=CommandSenderTest.js.map