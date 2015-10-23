define(["require", "exports", '../libs/CommandSender/bootstraper'], function (require, exports, commandSenderBootstraper) {
    $(function () {
        var x = commandSenderBootstraper;
        var sendCommand1 = function () {
            var command1 = {
                SourceAccountId: '7A8125D3-2BF9-45DE-A258-CE0D3C17892D',
                DestinationAccountId: '37EC9EA1-2A13-4A4D-B55E-6C844D822DAC',
                Amount: Math.random() * 1000
            };
            CK.CommandSender.send('TransferAmount', command1).done(function (r) {
                $("#results").append('<li class="long-running">' + JSON.stringify(r) + '</li>');
            });
        };
        var sendCommand2 = function () {
            var command2 = {
                AccountId: '7A8125D3-2BF9-45DE-A258-CE0D3C17892D',
                Amount: Math.random() * 1000
            };
            CK.CommandSender.send('WithdrawMoney', command2).done(function (r) {
                $("#results").append('<li>' + JSON.stringify(r) + '</li>');
            });
        };
        $("#btn1").click(function () {
            sendCommand1();
        });
        $("#btn2").click(function () {
            sendCommand2();
        });
        $("#btn3").click(function () {
            sendCommand1();
            sendCommand2();
        });
    });
});
//# sourceMappingURL=app.js.map