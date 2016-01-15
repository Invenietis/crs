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

	/// <reference path="../typings/tsd.d.ts" />
	var Ck = __webpack_require__(1);
	var test = __webpack_require__(6);
	var command_1 = __webpack_require__(7);
	describe("Action Handler Test", function () {
	    var activator = {
	        activate: function (type) {
	            if (type == test.TestHandler) {
	                return new test.TestHandler();
	            }
	        }
	    };
	    var resolver = new Ck.CommandResolver(activator);
	    resolver.registerHandler(test.TestHandler);
	    it("Handler should be executed", function (done) {
	        var sender = new Ck.ActionSender(resolver);
	        sender.send(new command_1.Command("test", {
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
	            sender.send(new command_1.Command("tests", {
	                a: 1,
	                b: 6
	            }));
	        }).toThrow("No handler found for the command tests");
	    });
	});


/***/ },
/* 1 */
/***/ function(module, exports, __webpack_require__) {

	function __export(m) {
	    for (var p in m) if (!exports.hasOwnProperty(p)) exports[p] = m[p];
	}
	__export(__webpack_require__(2));
	__export(__webpack_require__(3));
	__export(__webpack_require__(4));
	__export(__webpack_require__(5));


/***/ },
/* 2 */
/***/ function(module, exports) {

	function ActionHandler(commandName) {
	    return function (target) {
	        target.__cmd = commandName;
	        return target;
	    };
	}
	exports.ActionHandler = ActionHandler;


/***/ },
/* 3 */
/***/ function(module, exports) {

	var ActionSender = (function () {
	    function ActionSender(_resolver) {
	        this._resolver = _resolver;
	    }
	    ActionSender.prototype.send = function (command) {
	        var handler = this._resolver.resolve(command);
	        return handler.handle(command);
	    };
	    return ActionSender;
	})();
	exports.ActionSender = ActionSender;


/***/ },
/* 4 */
/***/ function(module, exports) {

	

/***/ },
/* 5 */
/***/ function(module, exports) {

	var CommandResolver = (function () {
	    function CommandResolver(_activator) {
	        this._activator = _activator;
	        this._handlers = {};
	    }
	    CommandResolver.prototype.registerHandler = function (handler) {
	        var h = handler;
	        if (!h.__cmd) {
	            throw "The handler " + handler + " has no associated command. Please use the ActionHandler decorator to specify one";
	        }
	        this._handlers[h.__cmd] = {
	            type: handler,
	            instance: undefined
	        };
	    };
	    CommandResolver.prototype.resolve = function (command) {
	        var handlerInfo = this._handlers[command.name];
	        if (handlerInfo == undefined)
	            throw "No handler found for the command " + command.name;
	        //create and store the handler instance
	        if (handlerInfo.instance == undefined) {
	            handlerInfo.instance = this._activator.activate(handlerInfo.type);
	        }
	        return handlerInfo.instance;
	    };
	    return CommandResolver;
	})();
	exports.CommandResolver = CommandResolver;


/***/ },
/* 6 */
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
	var Ck = __webpack_require__(1);
	var TestHandler = (function () {
	    function TestHandler() {
	    }
	    TestHandler.prototype.handle = function (action) {
	        console.log(action);
	        return Promise.resolve(action.properties.a + action.properties.b);
	    };
	    TestHandler = __decorate([
	        Ck.ActionHandler("test"), 
	        __metadata('design:paramtypes', [])
	    ], TestHandler);
	    return TestHandler;
	})();
	exports.TestHandler = TestHandler;


/***/ },
/* 7 */
/***/ function(module, exports, __webpack_require__) {

	function __export(m) {
	    for (var p in m) if (!exports.hasOwnProperty(p)) exports[p] = m[p];
	}
	__export(__webpack_require__(8));
	__export(__webpack_require__(9));
	__export(__webpack_require__(10));
	__export(__webpack_require__(11));
	__export(__webpack_require__(12));


/***/ },
/* 8 */
/***/ function(module, exports) {

	var Command = (function () {
	    function Command(name, properties) {
	        this.name = name;
	        this.properties = properties;
	    }
	    return Command;
	})();
	exports.Command = Command;


/***/ },
/* 9 */
/***/ function(module, exports) {

	var AjaxSender = (function () {
	    function AjaxSender() {
	    }
	    AjaxSender.prototype.send = function (url, command) {
	        var json = JSON.stringify(command.properties);
	        return new Promise(function (resolve, reject) {
	            $.ajax(url, {
	                type: 'POST',
	                data: json,
	                contentType: 'application/json',
	                dataType: 'JSON'
	            }).then(resolve, reject);
	        });
	    };
	    return AjaxSender;
	})();
	exports.AjaxSender = AjaxSender;


/***/ },
/* 10 */
/***/ function(module, exports) {

	var CommandEmitter = (function () {
	    function CommandEmitter(prefix, commandRequestSender, commandResponseListener) {
	        this._prefix = prefix;
	        this._sender = commandRequestSender;
	        this._listener = commandResponseListener;
	    }
	    CommandEmitter.prototype.emit = function (command) {
	        var _this = this;
	        console.info('Sending Command : ' + command.name);
	        var url = this._prefix + '/' + command.name + '?c=' + (this._listener ? this._listener.callbackId : '');
	        var xhr = this._sender.send(url, command);
	        return xhr.then(function (data) {
	            if (data !== null) {
	                switch (data.responseType) {
	                    case -1: throw new Error(data.payload);
	                    // Direct resposne
	                    case 0: return data;
	                    // Deferred response
	                    case 1: {
	                        if (_this._listener == null) {
	                            throw new Error("Deferred command execution is not supported by the Server. It should not answer a deferred response type.");
	                        }
	                        var callbackId = data.payload;
	                        return _this._listener.listen(data.commandId, callbackId);
	                    }
	                }
	            }
	        });
	    };
	    return CommandEmitter;
	})();
	exports.CommandEmitter = CommandEmitter;


/***/ },
/* 11 */
/***/ function(module, exports) {

	

/***/ },
/* 12 */
/***/ function(module, exports) {

	var SignalRListener = (function () {
	    function SignalRListener(connection, hubName) {
	        var _this = this;
	        this._receivedResponses = new Array();
	        this._hubConnection = connection;
	        this.callbackId = this._hubConnection.hub.id;
	        this._hubConnection.proxies[hubName].client.ReceiveCommandResponse = function (data) {
	            _this._receivedResponses.push(data);
	        };
	    }
	    SignalRListener.prototype.listen = function (commandId, callbackId) {
	        if (callbackId !== this._hubConnection.id)
	            throw new Error('Try to listen to the wrong ConnectionId...');
	        return new Promise(function (resolve, reject) {
	            var _this = this;
	            var interval = setInterval(function () {
	                _this._receivedResponses.forEach(function (r, idx, ar) {
	                    if (r.commandId === commandId) {
	                        clearInterval(interval);
	                        ar.splice(idx, 1);
	                        resolve(r);
	                    }
	                });
	            }, 200);
	        });
	    };
	    return SignalRListener;
	})();
	exports.SignalRListener = SignalRListener;


/***/ }
/******/ ]);