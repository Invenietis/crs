define(["require", "exports", 'commands'], function (require, exports, commands_1) {
    $.connection.hub.logging = true;
    var signalRListener = new commands_1.SignalRListener($.connection.hub, 'commandresponse');
    $.connection.hub.start().done(function (d) {
        CK.CommandSender = new commands_1.CommandSender('/c', $.connection.hub.id, new commands_1.AjaxSender(), signalRListener);
    }).fail(function (er) {
        throw new Error(er);
    });
});
//# sourceMappingURL=bootstraper.js.map