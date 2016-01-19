var SignalRListener = (function () {
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
})();
exports.SignalRListener = SignalRListener;
