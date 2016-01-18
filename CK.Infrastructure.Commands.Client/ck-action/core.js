System.register(['./src/ActionExecutor', './src/ActionInvoker', './src/Resolver'], function(exports_1) {
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
            function (ActionExecutor_1_1) {
                exportStar_1(ActionExecutor_1_1);
            },
            function (ActionInvoker_1_1) {
                exportStar_1(ActionInvoker_1_1);
            },
            function (Resolver_1_1) {
                exportStar_1(Resolver_1_1);
            }],
        execute: function() {
        }
    }
});
