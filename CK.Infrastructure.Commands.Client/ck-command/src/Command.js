System.register([], function(exports_1) {
    "use strict";
    var Command;
    return {
        setters:[],
        execute: function() {
            Command = (function () {
                function Command(name, properties) {
                    this.name = name;
                    this.properties = properties;
                }
                return Command;
            }());
            exports_1("Command", Command);
        }
    }
});
//# sourceMappingURL=Command.js.map