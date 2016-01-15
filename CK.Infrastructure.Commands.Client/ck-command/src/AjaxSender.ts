/// <reference path="../typings/tsd.d.ts" />
import { ICommandRequestSender } from "./Interfaces";
import {Command}  from "./Command";

export class AjaxSender implements ICommandRequestSender {
    
    send(url: string, command: Command) {
        var json = JSON.stringify(command.properties);
        
        return new Promise<any>(function(resolve, reject){
                $.ajax(url, {
                type: 'POST',
                data: json,
                contentType: 'application/json',
                dataType: 'JSON'
            }).then(resolve, reject);
        }) 
    } 
}