System.register(['../core'], function(exports_1) {
    "use strict";
    var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
        var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
        if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
        else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
        return c > 3 && r && Object.defineProperty(target, key, r), r;
    };
    var __metadata = (this && this.__metadata) || function (k, v) {
        if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
    };
    var Ck;
    var TestHandler, WrongHandler, DuplicateTestHandler, WrongTestHandler, DumbActivator;
    return {
        setters:[
            function (Ck_1) {
                Ck = Ck_1;
            }],
        execute: function() {
            TestHandler = (function () {
                function TestHandler() {
                }
                TestHandler.prototype.handle = function (action) {
                    return Promise.resolve(action.properties.a + action.properties.b);
                };
                TestHandler = __decorate([
                    Ck.ActionHandler("test"), 
                    __metadata('design:paramtypes', [])
                ], TestHandler);
                return TestHandler;
            }());
            exports_1("TestHandler", TestHandler);
            WrongHandler = (function () {
                function WrongHandler() {
                }
                WrongHandler.prototype.handle = function (action) {
                    return Promise.resolve(action.properties.a + action.properties.b);
                };
                return WrongHandler;
            }());
            exports_1("WrongHandler", WrongHandler);
            DuplicateTestHandler = (function () {
                function DuplicateTestHandler() {
                }
                DuplicateTestHandler.prototype.handle = function (action) {
                    return Promise.resolve(action.properties.a + action.properties.b);
                };
                DuplicateTestHandler = __decorate([
                    Ck.ActionHandler("test"), 
                    __metadata('design:paramtypes', [])
                ], DuplicateTestHandler);
                return DuplicateTestHandler;
            }());
            exports_1("DuplicateTestHandler", DuplicateTestHandler);
            WrongTestHandler = (function () {
                function WrongTestHandler() {
                }
                WrongTestHandler.prototype.doStuff = function () {
                };
                WrongTestHandler = __decorate([
                    Ck.ActionHandler("test"), 
                    __metadata('design:paramtypes', [])
                ], WrongTestHandler);
                return WrongTestHandler;
            }());
            exports_1("WrongTestHandler", WrongTestHandler);
            DumbActivator = (function () {
                function DumbActivator() {
                }
                DumbActivator.prototype.activate = function (type) {
                    if (type == TestHandler) {
                        return new TestHandler();
                    }
                };
                return DumbActivator;
            }());
            exports_1("DumbActivator", DumbActivator);
        }
    }
});
