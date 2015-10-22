
export interface ICommandResponseListener {
    listen: (commandId: String, callback: String) => JQueryPromise<any>;
}

export interface ICommandSender {
    /**
     * @param route  The specific command route name.
     * @param commandBody  The command payload DTO.
     * @param callback  A callback
     */
    send: (route: String, commandBody: Object, callback: Function) => JQueryPromise<any>;
}

export interface ICommandResponse {
    /**
    * The identifier of the command. */
    CommandId: String,
    /**
    * The payload of the result. */
    Payload: any,

    ResponseType: Number
}

export class CommandSender implements ICommandSender {
    private _listener: ICommandResponseListener

    constructor(commandResponseListener: ICommandResponseListener) {
        this._listener = commandResponseListener;
    }

    send(route: String, commandBody: Object, callback: Function) {
        console.info('Sending Command to route: ' + route);

        var xhr = $.ajax('/c/' + route, {
            type: 'POST',
            data: JSON.stringify(commandBody),
            contentType: 'application/json',
            dataType: 'JSON'
        });

        var deferred = $.Deferred();

        xhr.done((data, status, jqXhr) => {

            var o = JSON.parse(data);
            let a = o as ICommandResponse;
            if (a !== null) {
                switch (a.ResponseType) {
                    // Direct resposne
                    case 0: deferred.resolve(a.Payload); break;
                    // Deferred response
                    case 1: {
                        var callbackId: String = a.Payload;
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