/// <reference path="../libs/ck-command.d.ts" />
System.register(['angular2/core', 'ck-command/command', 'ck-qrs/angular2'], function(exports_1) {
    var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
        var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
        if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
        else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
        return c > 3 && r && Object.defineProperty(target, key, r), r;
    };
    var __metadata = (this && this.__metadata) || function (k, v) {
        if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
    };
    var core_1, command_1, angular2_1;
    var AppComponent;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (command_1_1) {
                command_1 = command_1_1;
            },
            function (angular2_1_1) {
                angular2_1 = angular2_1_1;
            }],
        execute: function() {
            AppComponent = (function () {
                function AppComponent(_emitter) {
                    this._emitter = _emitter;
                    this.user = {};
                    this.query = {
                        name: "userList"
                    };
                    this.model = {};
                }
                AppComponent.prototype.addUser = function () {
                    this._emitter.emit(new command_1.Command('addUser', this.user));
                };
                AppComponent = __decorate([
                    core_1.Component({
                        selector: 'ck-command',
                        templateUrl: 'app/app.html',
                        directives: [angular2_1.CkModelComponent]
                    }), 
                    __metadata('design:paramtypes', [command_1.CommandEmitter])
                ], AppComponent);
                return AppComponent;
            })();
            exports_1("AppComponent", AppComponent);
        }
    }
});
//# sourceMappingURL=app.js.map