System.register([], function(exports_1) {
    "use strict";
    function ActionHandler(actionName) {
        return function (target) {
            target.__cmd = actionName;
            return target;
        };
    }
    exports_1("ActionHandler", ActionHandler);
    return {
        setters:[],
        execute: function() {
        }
    }
});
