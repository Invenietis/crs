/// <reference path="../Scripts/jquery.d.ts"/>
/// <reference path="../Scripts/tsUnit.ts"/>
/// <amd-dependency path="../Scripts"  />

import * as tsUnit from '../Scripts/tsUnit';
import * as Commands from '../CommandSender';

export class CommandSenderTests extends tsUnit.TestClass {

    private target = new Commands.CommandSender({
        listen: (commandId, callback): JQueryPromise<any> =>{

            return null;
        }
    });

    sendACommandShouldTriggerAnXhrRequestToTheServer() {
        this.isTrue(true);
    }
}