export class AjaxSender implements ICommandRequestSender {
    post(url: string, command: any) {
        var json = JSON.stringify(command);
        return $.ajax(url, {
            type: 'POST', 
            data: json,
            contentType: 'application/json',
            dataType: 'JSON'
        });
    }
}

export class SignalRListener implements ICommandResponseListener {
    private _hubConnection: HubConnection; 
    private _receivedResponses: Array<ICommandResponse>;
     
    constructor(connection: HubConnection, hubName: string) {
        var me = this;
        this._receivedResponses = new Array<ICommandResponse>();
        this._hubConnection = connection;
        this._hubConnection.proxies[hubName].client.ReceiveCommandResponse = data => {
            me._receivedResponses.push(data);
        };
    }

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

export class CommandSender implements ICommandSender { 
    private _sender: ICommandRequestSender
    private _listener: ICommandResponseListener
    private _prefix: string
    private _connectionId: string

    constructor(prefix: string, connectionId: string, commandRequestSender: ICommandRequestSender, commandResponseListener: ICommandResponseListener) {
        this._prefix = prefix;
        this._connectionId = connectionId;
        this._sender = commandRequestSender || new AjaxSender();
        this._listener = commandResponseListener;
    }

    public send(route: string, commandBody: Object) {
        console.info('Sending Command to route: ' + route);

        var url = this._prefix + '/' + route + '?c=' + this._connectionId;
        var xhr = this._sender.post(url, commandBody);

        var deferred = $.Deferred<ICommandResponse>();

        xhr.done((data, status, jqXhr) => {

            var a = data as ICommandResponse;
            if (a !== null) {
                switch (a.ResponseType) {
                    // Direct resposne
                    case 0: deferred.resolve(a); break;
                    // Deferred response
                    case 1: {
                        var callbackId: string = a.Payload;
                        var promise = this._listener.listen(a.CommandId, callbackId);
                        promise.done((commandResponseBody) => {
                            deferred.resolve(commandResponseBody);
                        });
                        break;
                    }
                }
            }

        });
        xhr.fail((jqXhr, textStatus, errorThrown) => {

        });

        return deferred.promise();
    }
}
