/// <reference path="../typings/tsd.d.ts" />
var CK;
(function (CK) {
    var Infrastructure;
    (function (Infrastructure) {
        var CommandSender = (function () {
            function CommandSender(prefix, commandRequestSender, commandResponseListener) {
                this._prefix = prefix;
                this._sender = commandRequestSender;
                this._listener = commandResponseListener;
            }
            CommandSender.prototype.send = function (route, commandBody) {
                var _this = this;
                console.info('Sending Command to route: ' + route);
                var url = this._prefix + '/' + route + '?c=' + (this._listener ? this._listener.callbackId : '');
                var xhr = this._sender.post(url, commandBody);
                var deferred = $.Deferred();
                xhr.done(function (data, status, jqXhr) {
                    var a = data;
                    if (a !== null) {
                        switch (a.ResponseType) {
                            case -1:
                                throw new Error(a.Payload);
                            // Direct resposne
                            case 0:
                                deferred.resolve(a);
                                break;
                            // Deferred response
                            case 1: {
                                if (_this._listener == null) {
                                    throw new Error("Deferred command execution is not supported by the Server. It should not answer a deferred response type.");
                                }
                                var callbackId = a.Payload;
                                var promise = _this._listener.listen(a.CommandId, callbackId);
                                promise.done(function (commandResponseBody) {
                                    deferred.resolve(commandResponseBody);
                                });
                                break;
                            }
                        }
                    }
                });
                xhr.fail(function (jqXhr, textStatus, errorThrown) {
                    throw new Error(textStatus);
                });
                return deferred.promise();
            };
            return CommandSender;
        }());
        Infrastructure.CommandSender = CommandSender;
    })(Infrastructure = CK.Infrastructure || (CK.Infrastructure = {}));
})(CK || (CK = {}));
//# sourceMappingURL=CommandSender.js.map