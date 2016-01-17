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
	module.exports = __webpack_require__(8);


/***/ },
/* 1 */
/***/ function(module, exports, __webpack_require__) {

	/// <reference path="../typings/tsd.d.ts" />
	var Ck = __webpack_require__(2);
	var test = __webpack_require__(9);
	describe("Action Sender Test", function () {
	    var activator = new test.DumbActivator();
	    var resolver = new Ck.ActionResolver(activator);
	    resolver.registerExecutor(test.TestExecutor);
	    it("Executor should be executed", function (done) {
	        var sender = new Ck.ActionInvoker(resolver);
	        sender.invoke("test", {
	            a: 1,
	            b: 6
	        }).then(function (r) {
	            expect(r).toBe(7);
	            done();
	        });
	    });
	    it("ActionSender should not found the executor and throw an exception", function () {
	        var sender = new Ck.ActionInvoker(resolver);
	        return expect(function () {
	            sender.invoke("tests", {
	                a: 1,
	                b: 6
	            });
	        }).toThrow("No executor found for the action tests");
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


/***/ },
/* 3 */
/***/ function(module, exports) {

	/// <reference path="../typings/tsd.d.ts" />
	/**
	 * ActionExector decorator. This must be used on every executor class
	 * @param metadata { actionName The name of the targeted action }
	 */
	function ActionExecutor(metadata) {
	    return function (target) {
	        target.__meta = metadata;
	        return target;
	    };
	}
	exports.ActionExecutor = ActionExecutor;


/***/ },
/* 4 */
/***/ function(module, exports) {

	var ActionInvoker = (function () {
	    function ActionInvoker(_resolver) {
	        this._resolver = _resolver;
	    }
	    ActionInvoker.prototype.invoke = function (actionName, parameters) {
	        var handler = this._resolver.resolve(actionName);
	        return handler.execute(parameters);
	    };
	    return ActionInvoker;
	})();
	exports.ActionInvoker = ActionInvoker;


/***/ },
/* 5 */
/***/ function(module, exports) {

	

/***/ },
/* 6 */
/***/ function(module, exports) {

	var ActionResolver = (function () {
	    function ActionResolver(_activator) {
	        this._activator = _activator;
	        this._executors = {};
	    }
	    ActionResolver.prototype.registerExecutor = function (executor) {
	        var ex = executor;
	        if (!ex.__meta || !ex.__meta.actionName) {
	            throw "The executor " + ex.name + " has no associated action. Please use the ActionExecutor decorator to specify one";
	        }
	        if (typeof executor.prototype.execute != 'function') {
	            throw "The executor " + ex.name + " does not satisfy the IActionExecutor interface";
	        }
	        if (this._executors[ex.__meta.actionName] != undefined) {
	            if (this._executors[ex.__meta.actionName].type == executor) {
	                throw "The executor " + ex.name + " is already registered";
	            }
	            throw "Cannot register " + ex.name + ": The executor " + this._executors[ex.__meta.actionName].type.name + " is already registered for the action " + ex.__meta.actionName;
	        }
	        this._executors[ex.__meta.actionName] = {
	            type: executor,
	            instance: undefined
	        };
	    };
	    ActionResolver.prototype.resolve = function (actionName) {
	        var executorInfo = this._executors[actionName];
	        if (executorInfo == undefined)
	            throw "No executor found for the action " + actionName;
	        //create and store the executor instance
	        if (executorInfo.instance == undefined) {
	            executorInfo.instance = this._activator.activate(executorInfo.type);
	        }
	        return executorInfo.instance;
	    };
	    return ActionResolver;
	})();
	exports.ActionResolver = ActionResolver;


/***/ },
/* 7 */,
/* 8 */
/***/ function(module, exports, __webpack_require__) {

	/// <reference path="../typings/tsd.d.ts" />
	var Ck = __webpack_require__(2);
	var executors = __webpack_require__(9);
	describe("ActionResolver Tests", function () {
	    it("Resolver should not accept an executor without the ActionExecutor decorator", function () {
	        var resolver = new Ck.ActionResolver(new executors.DumbActivator());
	        return expect(function () {
	            resolver.registerExecutor(executors.WrongExecutor);
	        }).toThrow("The executor WrongExecutor has no associated action. Please use the ActionExecutor decorator to specify one");
	    });
	    it("Resolver should not accept an executor that not satisfy the IActionExecutor interface", function () {
	        var resolver = new Ck.ActionResolver(new executors.DumbActivator());
	        return expect(function () {
	            resolver.registerExecutor(executors.WrongTestExecutor);
	        }).toThrow("The executor WrongTestExecutor does not satisfy the IActionExecutor interface");
	    });
	    it("Resolver should not allow the registration of an executor more than once", function () {
	        var resolver = new Ck.ActionResolver(new executors.DumbActivator());
	        resolver.registerExecutor(executors.TestExecutor);
	        return expect(function () {
	            resolver.registerExecutor(executors.TestExecutor);
	        }).toThrow("The executor TestExecutor is already registered");
	    });
	    it("Resolver should not allow the registration of differents executors for the same action", function () {
	        var resolver = new Ck.ActionResolver(new executors.DumbActivator());
	        resolver.registerExecutor(executors.TestExecutor);
	        return expect(function () {
	            resolver.registerExecutor(executors.DuplicateTestExecutor);
	        }).toThrow("Cannot register DuplicateTestExecutor: The executor TestExecutor is already registered for the action test");
	    });
	    it("Resolver should resolve the previously registered executor", function () {
	        var resolver = new Ck.ActionResolver(new executors.DumbActivator());
	        resolver.registerExecutor(executors.TestExecutor);
	        var executor = resolver.resolve('test');
	        return expect(executor instanceof executors.TestExecutor).toBeTruthy();
	    });
	    it("Resolver should not found the executor", function () {
	        var resolver = new Ck.ActionResolver(new executors.DumbActivator());
	        return expect(function () {
	            resolver.resolve('test');
	        }).toThrow("No executor found for the action test");
	    });
	});


/***/ },
/* 9 */
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
	var TestExecutor = (function () {
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
	})();
	exports.TestExecutor = TestExecutor;
	var WrongExecutor = (function () {
	    function WrongExecutor() {
	    }
	    WrongExecutor.prototype.execute = function (parameters) {
	        return Promise.resolve(parameters.a + parameters.b);
	    };
	    return WrongExecutor;
	})();
	exports.WrongExecutor = WrongExecutor;
	var DuplicateTestExecutor = (function () {
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
	})();
	exports.DuplicateTestExecutor = DuplicateTestExecutor;
	var WrongTestExecutor = (function () {
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
	})();
	exports.WrongTestExecutor = WrongTestExecutor;
	var DumbActivator = (function () {
	    function DumbActivator() {
	    }
	    DumbActivator.prototype.activate = function (type) {
	        if (type == TestExecutor) {
	            return new TestExecutor();
	        }
	    };
	    return DumbActivator;
	})();
	exports.DumbActivator = DumbActivator;


/***/ }
/******/ ]);