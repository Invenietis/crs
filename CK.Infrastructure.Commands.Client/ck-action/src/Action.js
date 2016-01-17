System.register([], function(exports_1) {
    "use strict";
    var Action;
    return {
        setters:[],
        execute: function() {
            Action = (function () {
                function Action(name, properties) {
                    this.name = name;
                    this.properties = properties;
                }
                return Action;
            }());
            exports_1("Action", Action);
        }
    }
});
