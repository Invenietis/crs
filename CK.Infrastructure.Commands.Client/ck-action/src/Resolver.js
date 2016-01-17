System.register([], function(exports_1) {
    "use strict";
    var ActionResolver;
    return {
        setters:[],
        execute: function() {
            ActionResolver = (function () {
                function ActionResolver(_activator) {
                    this._activator = _activator;
                    this._handlers = {};
                }
                ActionResolver.prototype.registerHandler = function (handler) {
                    var h = handler;
                    if (!h.__cmd) {
                        throw "The handler " + handler.name + " has no associated action. Please use the ActionHandler decorator to specify one";
                    }
                    if (typeof handler.prototype.handle != 'function') {
                        throw "The handler " + handler.name + " does not satisfy the IActionHandler interface";
                    }
                    if (this._handlers[h.__cmd] != undefined) {
                        if (this._handlers[h.__cmd].type == handler) {
                            throw "The handler " + handler.name + " is already registered";
                        }
                        throw "Cannot register " + handler.name + ": The handler " + this._handlers[h.__cmd].type.name + " is already registered for the command " + h.__cmd;
                    }
                    this._handlers[h.__cmd] = {
                        type: handler,
                        instance: undefined
                    };
                };
                ActionResolver.prototype.resolve = function (actionName) {
                    var handlerInfo = this._handlers[actionName];
                    if (handlerInfo == undefined)
                        throw "No handler found for the action " + actionName;
                    //create and store the handler instance
                    if (handlerInfo.instance == undefined) {
                        handlerInfo.instance = this._activator.activate(handlerInfo.type);
                    }
                    return handlerInfo.instance;
                };
                return ActionResolver;
            }());
            exports_1("ActionResolver", ActionResolver);
        }
    }
});
