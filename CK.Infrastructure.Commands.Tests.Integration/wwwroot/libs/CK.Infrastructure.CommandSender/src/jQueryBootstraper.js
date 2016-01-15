/// <reference path="CommandSender.ts" />
/// <reference path="AjaxSender.ts" />
/// <reference path="SignalRListener.ts" />
/// <reference path="../typings/tsd.d.ts" />
var CK;
(function (CK) {
    var Infrastructure;
    (function (Infrastructure) {
        var ajaxSender = new Infrastructure.AjaxSender();
        if ($.connection) {
            $.CK.commandListener = new Infrastructure.SignalRListener($.connection.hub, 'commandresponse');
        }
        $.CK.getCommandSender = function (prefix) {
            if (!$.CK['__crscache']) {
                $.CK['__crscache'] = [];
            }
            var entry = $.CK['__crscache'][prefix];
            if (!entry) {
                entry = $.CK['__crscache'][prefix] = new Infrastructure.CommandSender(prefix, ajaxSender, $.CK.commandListener);
            }
            return entry;
        };
        if ($.connection) {
            $.connection.hub.logging = true;
            $.connection.hub.start();
        }
    })(Infrastructure = CK.Infrastructure || (CK.Infrastructure = {}));
})(CK || (CK = {}));
//# sourceMappingURL=jQueryBootstraper.js.map