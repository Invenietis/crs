System.register(['./src/ActionHandler', './src/ActionSender', './src/Resolver', './src/Action'], function(exports_1) {
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
            function (ActionHandler_1_1) {
                exportStar_1(ActionHandler_1_1);
            },
            function (ActionSender_1_1) {
                exportStar_1(ActionSender_1_1);
            },
            function (Resolver_1_1) {
                exportStar_1(Resolver_1_1);
            },
            function (Action_1_1) {
                exportStar_1(Action_1_1);
            }],
        execute: function() {
        }
    }
});
