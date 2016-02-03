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
    var TestExecutor, WrongExecutor, DuplicateTestExecutor, WrongTestExecutor, DumbActivator;
    return {
        setters:[
            function (Ck_1) {
                Ck = Ck_1;
            }],
        execute: function() {
            TestExecutor = (function () {
                function TestExecutor() {
                }
                TestExecutor.prototype.execute = function (parameters) {
                    return Promise.resolve(parameters.a + parameters.b);
                };
                TestExecutor = __decorate([
                    Ck.ActionExecutor({
                        actionName: "test"
                    }), 
                    __metadata('design:paramtypes', [])
                ], TestExecutor);
                return TestExecutor;
            }());
            exports_1("TestExecutor", TestExecutor);
            WrongExecutor = (function () {
                function WrongExecutor() {
                }
                WrongExecutor.prototype.execute = function (parameters) {
                    return Promise.resolve(parameters.a + parameters.b);
                };
                return WrongExecutor;
            }());
            exports_1("WrongExecutor", WrongExecutor);
            DuplicateTestExecutor = (function () {
                function DuplicateTestExecutor() {
                }
                DuplicateTestExecutor.prototype.execute = function (parameters) {
                    return Promise.resolve(parameters.a + parameters.b);
                };
                DuplicateTestExecutor = __decorate([
                    Ck.ActionExecutor({
                        actionName: "test"
                    }), 
                    __metadata('design:paramtypes', [])
                ], DuplicateTestExecutor);
                return DuplicateTestExecutor;
            }());
            exports_1("DuplicateTestExecutor", DuplicateTestExecutor);
            WrongTestExecutor = (function () {
                function WrongTestExecutor() {
                }
                WrongTestExecutor.prototype.doStuff = function () {
                };
                WrongTestExecutor = __decorate([
                    Ck.ActionExecutor({
                        actionName: "test"
                    }), 
                    __metadata('design:paramtypes', [])
                ], WrongTestExecutor);
                return WrongTestExecutor;
            }());
            exports_1("WrongTestExecutor", WrongTestExecutor);
            DumbActivator = (function () {
                function DumbActivator() {
                }
                DumbActivator.prototype.activate = function (type) {
                    if (type == TestExecutor) {
                        return new TestExecutor();
                    }
                };
                return DumbActivator;
            }());
            exports_1("DumbActivator", DumbActivator);
        }
    }
});
