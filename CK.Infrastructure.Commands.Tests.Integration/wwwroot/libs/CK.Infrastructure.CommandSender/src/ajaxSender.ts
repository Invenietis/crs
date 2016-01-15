/// <reference path="../typings/tsd.d.ts" />

module CK.Infrastructure {
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

}