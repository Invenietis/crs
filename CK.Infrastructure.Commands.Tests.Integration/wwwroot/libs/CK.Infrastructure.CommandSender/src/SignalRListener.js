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
        }());
        Infrastructure.SignalRListener = SignalRListener;
    })(Infrastructure = CK.Infrastructure || (CK.Infrastructure = {}));
})(CK || (CK = {}));
//# sourceMappingURL=SignalRListener.js.map