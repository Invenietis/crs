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
        })();
        Infrastructure.CommandSender = CommandSender;
    })(Infrastructure = CK.Infrastructure || (CK.Infrastructure = {}));
})(CK || (CK = {}));
/// <reference path="../typings/tsd.d.ts" />
var CK;
(function (CK) {
    var Infrastructure;
    (function (Infrastructure) {
        var SignalRListener = (function () {
            function SignalRListener(connection, hubName) {
                var me = this;
                this._receivedResponses = new Array();
                this._hubConnection = connection;
                this.callbackId = this._hubConnection.hub.id;
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
        Infrastructure.SignalRListener = SignalRListener;
    })(Infrastructure = CK.Infrastructure || (CK.Infrastructure = {}));
})(CK || (CK = {}));
/// <reference path="../typings/tsd.d.ts" />
var CK;
(function (CK) {
    var Infrastructure;
    (function (Infrastructure) {
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
        Infrastructure.AjaxSender = AjaxSender;
    })(Infrastructure = CK.Infrastructure || (CK.Infrastructure = {}));
})(CK || (CK = {}));
/// <reference path="CommandSender.ts" />
/// <reference path="AjaxSender.ts" />
/// <reference path="SignalRListener.ts" />
/// <reference path="../typings/tsd.d.ts" />
var CK;
(function (CK) {
    var Infrastructure;
    (function (Infrastructure) {
        var ajaxSender = new Infrastructure.AjaxSender();
        if ($.connection) {
            $.CK.commandListener = new Infrastructure.SignalRListener($.connection.hub, 'commandresponse');
        }
        $.CK.getCommandSender = function (prefix) {
            if (!$.CK['__crscache']) {
                $.CK['__crscache'] = [];
            }
            var entry = $.CK['__crscache'][prefix];
            if (!entry) {
                entry = $.CK['__crscache'][prefix] = new Infrastructure.CommandSender(prefix, ajaxSender, $.CK.commandListener);
            }
            return entry;
        };
        if ($.connection) {
            $.connection.hub.logging = true;
            $.connection.hub.start();
        }
    })(Infrastructure = CK.Infrastructure || (CK.Infrastructure = {}));
})(CK || (CK = {}));
//# sourceMappingURL=commandSender.js.map