/// <reference path="CommandSender.ts" />
/// <reference path="AjaxSender.ts" />
/// <reference path="SignalRListener.ts" />
/// <reference path="../typings/tsd.d.ts" />

module CK.Infrastructure {
    var ajaxSender: ICommandRequestSender = new AjaxSender();
    if ($.connection) {
        $.CK.commandListener = new SignalRListener($.connection.hub, 'commandresponse')
    }

    $.CK.getCommandSender = (prefix) => {
        if (!$.CK['__crscache']) {
            $.CK['__crscache'] = [];
        }
        var entry = $.CK['__crscache'][prefix];
        if (!entry) {
            entry = $.CK['__crscache'][prefix] = new CommandSender(prefix, ajaxSender, $.CK.commandListener);
        }
        return entry;
    };
    if ($.connection) {
        $.connection.hub.logging = true;
        $.connection.hub.start();
    }
}