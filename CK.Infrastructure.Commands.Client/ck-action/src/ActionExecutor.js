/// <reference path="../typings/tsd.d.ts" />
System.register([], function(exports_1) {
    "use strict";
    function ActionExecutor(metadata) {
        return function (target) {
            target.__meta = metadata;
            return target;
        };
    }
    exports_1("ActionExecutor", ActionExecutor);
    return {
        setters:[],
        execute: function() {
        }
    }
});
