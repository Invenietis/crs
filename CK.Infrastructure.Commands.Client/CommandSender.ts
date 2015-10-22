/// <reference path="Scripts/jquery.d.ts"/>

export interface ICommandResponseListener {
    listen: (commandId: string, callbackId: string) => JQueryPromise<ICommandResponse>;
}

export interface ICommandRequestSender {
    post: (url: string, command: any) => JQueryPromise<any>;
}

export interface ICommandSender {
    /**
     * @param route  The specific command route name.
     * @param commandBody  The command payload DTO.
     * @param callback  A callback
     */
    send: (route: string, commandBody: Object) => JQueryPromise<ICommandResponse>;
}

export interface ICommandResponse {
    /**
    * The identifier of the command. */
    CommandId: string,
    /**
    * The payload of the result. */
    Payload: any, 

    ResponseType: Number 
}

class AjaxSender implements ICommandRequestSender {
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

export class CommandSender implements ICommandSender {
    private _sender: ICommandRequestSender
    private _listener: ICommandResponseListener

    constructor(commandRequestSender: ICommandRequestSender, commandResponseListener: ICommandResponseListener) {
        this._sender = commandRequestSender || new AjaxSender();
        this._listener = commandResponseListener;
    }

    send(route: string, commandBody: Object) {
        console.info('Sending Command to route: ' + route);

        var xhr = this._sender.post('/c' + route, commandBody);

        var deferred = $.Deferred<ICommandResponse>();

        xhr.done((data, status, jqXhr) => {

            var o = JSON.parse(data);
            let a = o as ICommandResponse;
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
