/// <reference path="../libs/CommandSender/CommandSender.d.ts" />
/// <reference path="../libs/signalr.d.ts" />
/// <reference path="../libs/jquery.d.ts" />

$.connection.hub.logging = true;

var commandHub = $.connection["commandResponse"];

var listener = new  CommandR
commandHub.ReceiveCommandResponse = ( commandResponse ) => {

}

$.connection.hub.start().done(function () {
    console.log('hub connection open');
});