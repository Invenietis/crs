/// <reference path="../typings/tsd.d.ts" />

module CK.Infrastructure {
    export class SignalRListener implements ICommandResponseListener {
        private _hubConnection: HubConnection;
        private _receivedResponses: Array<ICommandResponse>;

        constructor(connection: HubConnection, hubName: string) {
            var me = this;
            this._receivedResponses = new Array<ICommandResponse>();
            this._hubConnection = connection;
            this.callbackId = this._hubConnection.hub.id;
            this._hubConnection.proxies[hubName].client.ReceiveCommandResponse = data => {
                me._receivedResponses.push(data);
            };
        }

        callbackId: string 

        listen(commandId: string, callbackId: string) {
            if (callbackId !== this._hubConnection.id)
                throw new Error('Try to listen to the wrong ConnectionId...');

            var me = this;
            var def = $.Deferred<ICommandResponse>();
            var interval = setInterval(() => {
                me._receivedResponses.forEach((r, idx, ar) => {
                    if (r.CommandId === commandId) {
                        clearInterval(interval);
                        ar.splice(idx, 1);
                        def.resolve(r);
                    }
                });
            }, 200);

            return def.promise();
        }
    }


}