/// <reference path="../typings/tsd.d.ts" />
var CK;
(function (CK) {
    var Infrastructure;
    (function (Infrastructure) {
        var AjaxSender = (function () {
            function AjaxSender() {
            }
            AjaxSender.prototype.post = function (url, command) {
                var json = JSON.stringify(command);
                return $.ajax(url, {
                    type: 'POST',
                    data: json,
                    contentType: 'application/json',
                    dataType: 'JSON'
                });
            };
            return AjaxSender;
        })();
        Infrastructure.AjaxSender = AjaxSender;
    })(Infrastructure = CK.Infrastructure || (CK.Infrastructure = {}));
})(CK || (CK = {}));
//# sourceMappingURL=ajaxSender.js.map