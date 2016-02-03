/// <reference path="../typings/tsd.d.ts" />
var AjaxSender = (function () {
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
})();
exports.AjaxSender = AjaxSender;
