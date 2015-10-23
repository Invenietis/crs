/// <reference path="commands.ts" />
var CK;
(function (CK) {
    var Infrastructure;
    (function (Infrastructure) {
        $.connection.hub.logging = true;
        var signalRListener = new Infrastructure.SignalRListener($.connection.hub, 'commandresponse');
        $.connection.hub.start().done(function (d) {
            $.CK.commandSender = new Infrastructure.CommandSender('/c', $.connection.hub.id, new Infrastructure.AjaxSender(), signalRListener);
        }).fail(function (er) {
            throw new Error(er);
        });
    })(Infrastructure = CK.Infrastructure || (CK.Infrastructure = {}));
})(CK || (CK = {}));
