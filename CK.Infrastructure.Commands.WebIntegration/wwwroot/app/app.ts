/// <reference path="../libs/CommandSender/CK.Infrastructure.Commands.ts" />
/// <reference path="../libs/signalr.d.ts" />
/// <reference path="../libs/jquery.d.ts" />

import {CommandSender, AjaxSender, SignalRListener} from '../libs/CommandSender/CK.Infrastructure.Commands';

$.connection.hub.logging = true;

var signalRListener = new SignalRListener($.connection.hub, 'commandresponse')
$.connection.hub.start().done(function () {
    console.log('hub connection open');

    var sender = new CommandSender('/c', $.connection.hub.id, new AjaxSender(), signalRListener );
    var command = {
        SourceAccountId: '7A8125D3-2BF9-45DE-A258-CE0D3C17892D',
        DestinationAccountId: '37EC9EA1-2A13-4A4D-B55E-6C844D822DAC',
        Amount: '500'
    };

    sender.send('/TransferAmount', command).done(r => {
        console.log(r.Payload);
    });
});