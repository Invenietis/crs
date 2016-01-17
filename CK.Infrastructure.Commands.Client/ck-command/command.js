System.register(['./src/Command', './src/AjaxSender', './src/CommandEmitter', './src/SignalRListener'], function(exports_1) {
    "use strict";
    function exportStar_1(m) {
        var exports = {};
        for(var n in m) {
            if (n !== "default") exports[n] = m[n];
        }
        exports_1(exports);
    }
    return {
        setters:[
            function (Command_1_1) {
                exportStar_1(Command_1_1);
            },
            function (AjaxSender_1_1) {
                exportStar_1(AjaxSender_1_1);
            },
            function (CommandEmitter_1_1) {
                exportStar_1(CommandEmitter_1_1);
            },
            function (SignalRListener_1_1) {
                exportStar_1(SignalRListener_1_1);
            }],
        execute: function() {
        }
    }
});
