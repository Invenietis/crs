System.register([], function(exports_1) {
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
            exports_1("CommandEmitter", CommandEmitter);
        }
    }
});
//# sourceMappingURL=CommandEmitter.js.map