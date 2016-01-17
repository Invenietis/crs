/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};

/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {

/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId])
/******/ 			return installedModules[moduleId].exports;

/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			exports: {},
/******/ 			id: moduleId,
/******/ 			loaded: false
/******/ 		};

/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);

/******/ 		// Flag the module as loaded
/******/ 		module.loaded = true;

/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}


/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;

/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;

/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";

/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(0);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/***/ function(module, exports, __webpack_require__) {

	__webpack_require__(1);
	module.exports = __webpack_require__(9);


/***/ },
/* 1 */
/***/ function(module, exports, __webpack_require__) {

	/// <reference path="../typings/tsd.d.ts" />
	var Ck = __webpack_require__(2);
	var test = __webpack_require__(8);
	describe("Action Sender Test", function () {
	    var activator = new test.DumbActivator();
	    var resolver = new Ck.ActionResolver(activator);
	    resolver.registerHandler(test.TestHandler);
	    it("Handler should be executed", function (done) {
	        var sender = new Ck.ActionSender(resolver);
	        sender.send(new Ck.Action("test", {
	            a: 1,
	            b: 6
	        })).then(function (r) {
	            expect(r).toBe(7);
	            done();
	        });
	    });
	    it("ActionSender should not found the handler and throw an exception", function () {
	        var sender = new Ck.ActionSender(resolver);
	        return expect(function () {
	            sender.send(new Ck.Action("tests", {
	                a: 1,
	                b: 6
	            }));
	        }).toThrow("No handler found for the action tests");
	    });
	});


/***/ },
/* 2 */
/***/ function(module, exports, __webpack_require__) {

	function __export(m) {
	    for (var p in m) if (!exports.hasOwnProperty(p)) exports[p] = m[p];
	}
	__export(__webpack_require__(3));
	__export(__webpack_require__(4));
	__export(__webpack_require__(5));
	__export(__webpack_require__(6));
	__export(__webpack_require__(7));


/***/ },
/* 3 */
/***/ function(module, exports) {

	function ActionHandler(actionName) {
	    return function (target) {
	        target.__cmd = actionName;
	        return target;
	    };
	}
	exports.ActionHandler = ActionHandler;


/***/ },
/* 4 */
/***/ function(module, exports) {

	var ActionSender = (function () {
	    function ActionSender(_resolver) {
	        this._resolver = _resolver;
	    }
	    ActionSender.prototype.send = function (action) {
	        var handler = this._resolver.resolve(action.name);
	        return handler.handle(action);
	    };
	    return ActionSender;
	})();
	exports.ActionSender = ActionSender;


/***/ },
/* 5 */
/***/ function(module, exports) {

	

/***/ },
/* 6 */
/***/ function(module, exports) {

	var ActionResolver = (function () {
	    function ActionResolver(_activator) {
	        this._activator = _activator;
	        this._handlers = {};
	    }
	    ActionResolver.prototype.registerHandler = function (handler) {
	        var h = handler;
	        if (!h.__cmd) {
	            throw "The handler " + handler.name + " has no associated action. Please use the ActionHandler decorator to specify one";
	        }
	        if (typeof handler.prototype.handle != 'function') {
	            throw "The handler " + handler.name + " does not satisfy the IActionHandler interface";
	        }
	        if (this._handlers[h.__cmd] != undefined) {
	            if (this._handlers[h.__cmd].type == handler) {
	                throw "The handler " + handler.name + " is already registered";
	            }
	            throw "Cannot register " + handler.name + ": The handler " + this._handlers[h.__cmd].type.name + " is already registered for the command " + h.__cmd;
	        }
	        this._handlers[h.__cmd] = {
	            type: handler,
	            instance: undefined
	        };
	    };
	    ActionResolver.prototype.resolve = function (actionName) {
	        var handlerInfo = this._handlers[actionName];
	        if (handlerInfo == undefined)
	            throw "No handler found for the action " + actionName;
	        //create and store the handler instance
	        if (handlerInfo.instance == undefined) {
	            handlerInfo.instance = this._activator.activate(handlerInfo.type);
	        }
	        return handlerInfo.instance;
	    };
	    return ActionResolver;
	})();
	exports.ActionResolver = ActionResolver;


/***/ },
/* 7 */
/***/ function(module, exports) {

	var Action = (function () {
	    function Action(name, properties) {
	        this.name = name;
	        this.properties = properties;
	    }
	    return Action;
	})();
	exports.Action = Action;


/***/ },
/* 8 */
/***/ function(module, exports, __webpack_require__) {

	var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
	    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
	    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
	    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
	    return c > 3 && r && Object.defineProperty(target, key, r), r;
	};
	var __metadata = (this && this.__metadata) || function (k, v) {
	    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
	};
	var Ck = __webpack_require__(2);
	var TestHandler = (function () {
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
	})();
	exports.TestHandler = TestHandler;
	var WrongHandler = (function () {
	    function WrongHandler() {
	    }
	    WrongHandler.prototype.handle = function (action) {
	        return Promise.resolve(action.properties.a + action.properties.b);
	    };
	    return WrongHandler;
	})();
	exports.WrongHandler = WrongHandler;
	var DuplicateTestHandler = (function () {
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
	})();
	exports.DuplicateTestHandler = DuplicateTestHandler;
	var WrongTestHandler = (function () {
	    function WrongTestHandler() {
	    }
	    WrongTestHandler.prototype.doStuff = function () {
	    };
	    WrongTestHandler = __decorate([
	        Ck.ActionHandler("test"), 
	        __metadata('design:paramtypes', [])
	    ], WrongTestHandler);
	    return WrongTestHandler;
	})();
	exports.WrongTestHandler = WrongTestHandler;
	var DumbActivator = (function () {
	    function DumbActivator() {
	    }
	    DumbActivator.prototype.activate = function (type) {
	        if (type == TestHandler) {
	            return new TestHandler();
	        }
	    };
	    return DumbActivator;
	})();
	exports.DumbActivator = DumbActivator;


/***/ },
/* 9 */
/***/ function(module, exports, __webpack_require__) {

	/// <reference path="../typings/tsd.d.ts" />
	var Ck = __webpack_require__(2);
	var handlers = __webpack_require__(8);
	describe("ActionResolver Tests", function () {
	    it("Resolver should not accept a handler without the ActionHandler decorator", function () {
	        var resolver = new Ck.ActionResolver(new handlers.DumbActivator());
	        return expect(function () {
	            resolver.registerHandler(handlers.WrongHandler);
	        }).toThrow("The handler WrongHandler has no associated action. Please use the ActionHandler decorator to specify one");
	    });
	    it("Resolver should not accept a handler that not satisfy the IActionHandler interface", function () {
	        var resolver = new Ck.ActionResolver(new handlers.DumbActivator());
	        return expect(function () {
	            resolver.registerHandler(handlers.WrongTestHandler);
	        }).toThrow("The handler WrongTestHandler does not satisfy the IActionHandler interface");
	    });
	    it("Resolver should not allow the registration of a handler more than once", function () {
	        var resolver = new Ck.ActionResolver(new handlers.DumbActivator());
	        resolver.registerHandler(handlers.TestHandler);
	        return expect(function () {
	            resolver.registerHandler(handlers.TestHandler);
	        }).toThrow("The handler TestHandler is already registered");
	    });
	    it("Resolver should not allow the registration of differents handlers for the same action", function () {
	        var resolver = new Ck.ActionResolver(new handlers.DumbActivator());
	        resolver.registerHandler(handlers.TestHandler);
	        return expect(function () {
	            resolver.registerHandler(handlers.DuplicateTestHandler);
	        }).toThrow("Cannot register DuplicateTestHandler: The handler TestHandler is already registered for the command test");
	    });
	    it("Resolver should resolve the previously registered handler", function () {
	        var resolver = new Ck.ActionResolver(new handlers.DumbActivator());
	        resolver.registerHandler(handlers.TestHandler);
	        var handler = resolver.resolve('test');
	        return expect(handler instanceof handlers.TestHandler).toBeTruthy();
	    });
	    it("Resolver should not found the handler", function () {
	        var resolver = new Ck.ActionResolver(new handlers.DumbActivator());
	        return expect(function () {
	            resolver.resolve('test');
	        }).toThrow("No handler found for the action test");
	    });
	});


/***/ }
/******/ ]);