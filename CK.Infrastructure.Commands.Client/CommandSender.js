var CommandSender = (function () {
    function CommandSender(commandResponseListener) {
        this._listener = commandResponseListener;
    }
    CommandSender.prototype.send = function (route, commandBody, callback) {
        var _this = this;
        console.info('Sending Command to route: ' + route);
        var xhr = $.ajax('/c/' + route, {
            type: 'POST',
            data: JSON.stringify(commandBody),
            contentType: 'application/json',
            dataType: 'JSON'
        });
        var deferred = $.Deferred();
        xhr.done(function (data, status, jqXhr) {
            var o = JSON.parse(data);
            var a = o;
            if (a !== null) {
                switch (a.ResponseType) {
                    // Direct resposne
                    case 0:
                        deferred.resolve(a.Payload);
                        break;
                    // Deferred response
                    case 1: {
                        var callbackId = a.Payload;
                        var promise = _this._listener.listen(a.CommandId, callbackId);
                        promise.done(function (commandResponseBody) {
                            deferred.resolve(commandResponseBody);
                        });
                        break;
                    }
                }
            }
        });
        xhr.fail(function (jqXhr, textStatus, errorThrown) {
        });
        return deferred.promise();
    };
    return CommandSender;
})();
exports.CommandSender = CommandSender;
