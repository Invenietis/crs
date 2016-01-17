System.register([], function(exports_1) {
    "use strict";
    var ActionSender;
    return {
        setters:[],
        execute: function() {
            ActionSender = (function () {
                function ActionSender(_resolver) {
                    this._resolver = _resolver;
                }
                ActionSender.prototype.send = function (action) {
                    var handler = this._resolver.resolve(action.name);
                    return handler.handle(action);
                };
                return ActionSender;
            }());
            exports_1("ActionSender", ActionSender);
        }
    }
});
