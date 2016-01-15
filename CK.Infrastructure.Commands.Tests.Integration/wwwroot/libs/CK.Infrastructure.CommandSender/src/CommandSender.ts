/// <reference path="../typings/tsd.d.ts" />

module CK.Infrastructure {

    export class CommandSender implements ICommandSender {
        private _sender: ICommandRequestSender
        private _listener: ICommandResponseListener
        private _prefix: string

        constructor(prefix: string, commandRequestSender: ICommandRequestSender, commandResponseListener?: ICommandResponseListener) {
            this._prefix = prefix;
            this._sender = commandRequestSender;
            this._listener = commandResponseListener;
        }

        public send(route: string, commandBody: Object) {
            console.info('Sending Command to route: ' + route);

            var url = this._prefix + '/' + route + '?c=' + (this._listener ? this._listener.callbackId : '');
            var xhr = this._sender.post(url, commandBody);

            var deferred = $.Deferred<ICommandResponse>();

            xhr.done((data, status, jqXhr) => {

                var a = data as ICommandResponse;
                if (a !== null) {
                    switch (a.ResponseType) {
                        case -1:
                            throw new Error(a.Payload);
                            
                        // Direct resposne
                        case 0: deferred.resolve(a); break; 
                        // Deferred response
                        case 1: {
                            if (this._listener == null) {
                                throw new Error("Deferred command execution is not supported by the Server. It should not answer a deferred response type.");
                            }

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
                throw new Error(textStatus);
            });

            return deferred.promise();
        }
    }

}
