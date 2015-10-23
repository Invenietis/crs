import {CommandSender, AjaxSender, SignalRListener} from './commands';

$.connection.hub.logging = true;
var signalRListener = new SignalRListener($.connection.hub, 'commandresponse')
$.connection.hub.start().done((d) => {
    CK.CommandSender = new CommandSender('/c', $.connection.hub.id, new AjaxSender(), signalRListener);
}).fail((er) => {
    throw new Error(er);
});
 