System.register([], function(exports_1) {
    "use strict";
    var ActionInvoker;
    return {
        setters:[],
        execute: function() {
            /**
             * Responsible of action invocation and execution
             */
            ActionInvoker = (function () {
                function ActionInvoker(_resolver) {
                    this._resolver = _resolver;
                }
                /**
                 * Invoke the specified action
                 * @param actionName The action to invoke
                 * @param parameters The action parameters
                 * @return A promise that will wrap the action execution result
                 */
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
