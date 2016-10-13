/// <reference path="../libs/ck-command.d.ts" />

// $(function () {
//     var sendCommand1 = function () {
//         
//         CK.CRS.sendCommand('c/admin', 'transfer', {
//             SourceAccountId: '7A8125D3-2BF9-45DE-A258-CE0D3C17892D',
//             DestinationAccountId: '37EC9EA1-2A13-4A4D-B55E-6C844D822DAC',
//             Amount: Math.random() * 1000
//         }).then(r => {
//             $("#results").append('<li class="long-running">' + JSON.stringify(r) + '</li>');
//             }).catch(err => console.error(err));
//     }; 
//     var sendCommand2 = function () {
// 
//         CK.CRS.sendCommand('c/admin', 'withdraw', {
//             AccountId: '7A8125D3-2BF9-45DE-A258-CE0D3C17892D',
//             Amount: Math.random() * 1000
//         }).then(r => {
//             $("#results").append('<li>' + JSON.stringify(r) + '</li>');
//             }).catch(err => console.error(err));
//     }; 
//     
//     $("#btn1").click(function () { 
//         sendCommand1();
//     });
//     $("#btn2").click(function () { 
//         sendCommand2();
//     });
//     $("#btn3").click(function () {
//         sendCommand1();
//         sendCommand2();
//     }); 
// });

import { Component } from 'angular2/core';
import { CommandEmitter, Command } from 'ck-crs/core';
import {CkModelComponent} from 'ck-qrs/angular2'; 

@Component({
    selector: 'ck-command',
    templateUrl: 'app/app.html',
    directives: [CkModelComponent]
})
export class AppComponent {
    user: any = {};
    query: any= {
        name: "userList"
    };
    model: any = {};

    constructor(private _emitter: CommandEmitter) {
        
    }

    addUser() {
        this._emitter.emit(new Command('addUser', this.user));
    }
} 
