System.register("src/Command", [], function(exports_1) {
    "use strict";
    var Command;
    return {
        setters:[],
        execute: function() {
            Command = (function () {
                function Command(name, properties) {
                    this.name = name;
                    properties = this.properties;
                }
                return Command;
            }());
            exports_1("Command", Command);
        }
    }
});
System.register("src/Interfaces", [], function(exports_2) {
    "use strict";
    return {
        setters:[],
        execute: function() {
        }
    }
});
System.register("src/AjaxSender", [], function(exports_3) {
    "use strict";
    var AjaxSender;
    return {
        setters:[],
        execute: function() {
            AjaxSender = (function () {
                function AjaxSender() {
                }
                AjaxSender.prototype.send = function (url, command) {
                    var json = JSON.stringify(command.properties);
                    return new Promise(function (resolve, reject) {
                        $.ajax(url, {
                            type: 'POST',
                            data: json,
                            contentType: 'application/json',
                            dataType: 'JSON'
                        }).then(resolve, reject);
                    });
                };
                return AjaxSender;
            }());
            exports_3("AjaxSender", AjaxSender);
        }
    }
});
System.register("src/CommandEmitter", [], function(exports_4) {
    "use strict";
    var CommandEmitter;
    return {
        setters:[],
        execute: function() {
            CommandEmitter = (function () {
                function CommandEmitter(prefix, commandRequestSender, commandResponseListener) {
                    this._prefix = prefix;
                    this._sender = commandRequestSender;
                    this._listener = commandResponseListener;
                }
                CommandEmitter.prototype.emit = function (command) {
                    var _this = this;
                    console.info('Sending Command : ' + command.name);
                    var url = this._prefix + '/' + command.name + '?c=' + (this._listener ? this._listener.callbackId : '');
                    var xhr = this._sender.send(url, command);
                    return xhr.then(function (data) {
                        if (data !== null) {
                            switch (data.responseType) {
                                case -1: throw new Error(data.payload);
                                // Direct resposne
                                case 0: return data;
                                // Deferred response
                                case 1: {
                                    if (_this._listener == null) {
                                        throw new Error("Deferred command execution is not supported by the Server. It should not answer a deferred response type.");
                                    }
                                    var callbackId = data.payload;
                                    return _this._listener.listen(data.commandId, callbackId);
                                }
                            }
                        }
                    });
                };
                return CommandEmitter;
            }());
            exports_4("CommandEmitter", CommandEmitter);
        }
    }
});
System.register("src/SignalRListener", [], function(exports_5) {
    "use strict";
    var SignalRListener;
    return {
        setters:[],
        execute: function() {
            SignalRListener = (function () {
                function SignalRListener(connection, hubName) {
                    var _this = this;
                    this._receivedResponses = new Array();
                    this._hubConnection = connection;
                    this.callbackId = this._hubConnection.hub.id;
                    this._hubConnection.proxies[hubName].client.ReceiveCommandResponse = function (data) {
                        _this._receivedResponses.push(data);
                    };
                }
                SignalRListener.prototype.listen = function (commandId, callbackId) {
                    if (callbackId !== this._hubConnection.id)
                        throw new Error('Try to listen to the wrong ConnectionId...');
                    return new Promise(function (resolve, reject) {
                        var _this = this;
                        var interval = setInterval(function () {
                            _this._receivedResponses.forEach(function (r, idx, ar) {
                                if (r.commandId === commandId) {
                                    clearInterval(interval);
                                    ar.splice(idx, 1);
                                    resolve(r);
                                }
                            });
                        }, 200);
                    });
                };
                return SignalRListener;
            }());
            exports_5("SignalRListener", SignalRListener);
        }
    }
});
System.register("command", ["src/Command", "src/AjaxSender", "src/CommandEmitter"], function(exports_6) {
    "use strict";
    function exportStar_1(m) {
        var exports = {};
        for(var n in m) {
            if (n !== "default") exports[n] = m[n];
        }
        exports_6(exports);
    }
    return {
        setters:[
            function (Command_1_1) {
                exportStar_1(Command_1_1);
            },
            function (AjaxSender_1_1) {
                exportStar_1(AjaxSender_1_1);
            },
            function (CommandEmitter_1_1) {
                exportStar_1(CommandEmitter_1_1);
            }],
        execute: function() {
        }
    }
});
//# sourceMappingURL=ck-command.system.js.map