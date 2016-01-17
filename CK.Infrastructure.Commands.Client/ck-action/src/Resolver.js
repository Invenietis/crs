System.register([], function(exports_1) {
    "use strict";
    var ActionResolver;
    return {
        setters:[],
        execute: function() {
            ActionResolver = (function () {
                function ActionResolver(_activator) {
                    this._activator = _activator;
                    this._executors = {};
                }
                ActionResolver.prototype.registerExecutor = function (executor) {
                    var ex = executor;
                    if (!ex.__meta || !ex.__meta.actionName) {
                        throw "The executor " + ex.name + " has no associated action. Please use the ActionExecutor decorator to specify one";
                    }
                    if (typeof executor.prototype.execute != 'function') {
                        throw "The executor " + ex.name + " does not satisfy the IActionExecutor interface";
                    }
                    if (this._executors[ex.__meta.actionName] != undefined) {
                        if (this._executors[ex.__meta.actionName].type == executor) {
                            throw "The executor " + ex.name + " is already registered";
                        }
                        throw "Cannot register " + ex.name + ": The executor " + this._executors[ex.__meta.actionName].type.name + " is already registered for the action " + ex.__meta.actionName;
                    }
                    this._executors[ex.__meta.actionName] = {
                        type: executor,
                        instance: undefined
                    };
                };
                ActionResolver.prototype.resolve = function (actionName) {
                    var executorInfo = this._executors[actionName];
                    if (executorInfo == undefined)
                        throw "No executor found for the action " + actionName;
                    //create and store the executor instance
                    if (executorInfo.instance == undefined) {
                        executorInfo.instance = this._activator.activate(executorInfo.type);
                    }
                    return executorInfo.instance;
                };
                return ActionResolver;
            }());
            exports_1("ActionResolver", ActionResolver);
        }
    }
});
