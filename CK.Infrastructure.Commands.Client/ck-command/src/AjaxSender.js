System.register([], function(exports_1) {
    "use strict";
    var AjaxSender;
    return {
        setters:[],
        execute: function() {
            AjaxSender = (function () {
                function AjaxSender() {
                }
                AjaxSender.prototype.send = function (url, command) {
                    var json = JSON.stringify(command.properties);
                    return new Promise(function (resolve, reject) {
                        $.ajax(url, {
                            type: 'POST',
                            data: json,
                            contentType: 'application/json',
                            dataType: 'JSON'
                        }).then(resolve, reject);
                    });
                };
                return AjaxSender;
            }());
            exports_1("AjaxSender", AjaxSender);
        }
    }
});
//# sourceMappingURL=AjaxSender.js.map