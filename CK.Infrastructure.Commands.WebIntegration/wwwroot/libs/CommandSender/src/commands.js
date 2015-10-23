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
    exports.AjaxSender = AjaxSender;
    var SignalRListener = (function () {
        function SignalRListener(connection, hubName) {
            var me = this;
            this._receivedResponses = new Array();
            this._hubConnection = connection;
            this._hubConnection.proxies[hubName].client.ReceiveCommandResponse = function (data) {
                me._receivedResponses.push(data);
            };
        }
        SignalRListener.prototype.listen = function (commandId, callbackId) {
            if (callbackId !== this._hubConnection.id)
                throw new Error('Try to listen to the wrong ConnectionId...');
            var me = this;
            var def = $.Deferred();
            var interval = setInterval(function () {
                me._receivedResponses.forEach(function (r, idx, ar) {
                    if (r.CommandId === commandId) {
                        clearInterval(interval);
                        ar.splice(idx, 1);
                        def.resolve(r);
                    }
                });
            }, 200);
            return def.promise();
        };
        return SignalRListener;
    })();
    exports.SignalRListener = SignalRListener;
    var CommandSender = (function () {
        function CommandSender(prefix, connectionId, commandRequestSender, commandResponseListener) {
            this._prefix = prefix;
            this._connectionId = connectionId;
            this._sender = commandRequestSender || new AjaxSender();
            this._listener = commandResponseListener;
        }
        CommandSender.prototype.send = function (route, commandBody) {
            var _this = this;
            console.info('Sending Command to route: ' + route);
            var url = this._prefix + '/' + route + '?c=' + this._connectionId;
            var xhr = this._sender.post(url, commandBody);
            var deferred = $.Deferred();
            xhr.done(function (data, status, jqXhr) {
                var a = data;
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
//# sourceMappingURL=commands.js.map