/// <reference path="Scripts/jquery.d.ts"/>
define(["require", "exports"], function (require, exports) {
    var AjaxSender = (function () {
        function AjaxSender() {
        }
        AjaxSender.prototype.post = function (url, command) {
            var json = JSON.stringify(command);
            return $.ajax(url, {
                type: 'POST',
                data: json,
                contentType: 'application/json',
                dataType: 'JSON'
            });
        };
        return AjaxSender;
    })();
    var CommandSender = (function () {
        function CommandSender(commandRequestSender, commandResponseListener) {
            this._sender = commandRequestSender || new AjaxSender();
            this._listener = commandResponseListener;
        }
        CommandSender.prototype.send = function (route, commandBody) {
            var _this = this;
            console.info('Sending Command to route: ' + route);
            var xhr = this._sender.post('/c' + route, commandBody);
            var deferred = $.Deferred();
            xhr.done(function (data, status, jqXhr) {
                var o = JSON.parse(data);
                var a = o;
                if (a !== null) {
                    switch (a.ResponseType) {
                        // Direct resposne
                        case 0:
                            deferred.resolve(a);
                            break;
                        // Deferred response
                        case 1: {
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
            });
            return deferred.promise();
        };
        return CommandSender;
    })();
    exports.CommandSender = CommandSender;
});
//# sourceMappingURL=CommandSender.js.map