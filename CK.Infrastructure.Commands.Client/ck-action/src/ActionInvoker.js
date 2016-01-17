System.register([], function(exports_1) {
    "use strict";
    var ActionInvoker;
    return {
        setters:[],
        execute: function() {
            ActionInvoker = (function () {
                function ActionInvoker(_resolver) {
                    this._resolver = _resolver;
                }
                ActionInvoker.prototype.invoke = function (actionName, parameters) {
                    var handler = this._resolver.resolve(actionName);
                    return handler.execute(parameters);
                };
                return ActionInvoker;
            }());
            exports_1("ActionInvoker", ActionInvoker);
        }
    }
});
