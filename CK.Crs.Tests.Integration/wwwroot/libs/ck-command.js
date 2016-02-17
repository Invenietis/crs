var CRS =
/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId])
/******/ 			return installedModules[moduleId].exports;
/******/
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			exports: {},
/******/ 			id: moduleId,
/******/ 			loaded: false
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.loaded = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(0);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/***/ function(module, exports, __webpack_require__) {

	"use strict";
	function __export(m) {
	    for (var p in m) if (!exports.hasOwnProperty(p)) exports[p] = m[p];
	}
	__export(__webpack_require__(1));
	__export(__webpack_require__(2));
	__export(__webpack_require__(6));
	__export(__webpack_require__(3));
	__export(__webpack_require__(7));
	__export(__webpack_require__(4));
	__export(__webpack_require__(8));
	__export(__webpack_require__(9));


/***/ },
/* 1 */
/***/ function(module, exports) {

	"use strict";
	var Command = (function () {
	    function Command(name, properties) {
	        this.name = name;
	        this.properties = properties;
	    }
	    return Command;
	}());
	exports.Command = Command;


/***/ },
/* 2 */
/***/ function(module, exports, __webpack_require__) {

	/// <reference path="../../typings/tsd.d.ts" />
	"use strict";
	var Abstraction_1 = __webpack_require__(3);
	var HttpListener_1 = __webpack_require__(4);
	var AjaxSender = (function () {
	    function AjaxSender(_listener) {
	        this._listener = _listener;
	        if (this._listener == undefined) {
	            this._listener = new HttpListener_1.HttpListener();
	        }
	    }
	    AjaxSender.prototype.send = function (url, command) {
	        var _this = this;
	        var json = JSON.stringify(command.properties);
	        return new Promise(function (resolve, reject) {
	            $.ajax(url, {
	                type: 'POST',
	                data: json,
	                contentType: 'application/json',
	                dataType: 'JSON'
	            }).then(function (data) {
	                var resp = new Abstraction_1.CommandResponse(data, command.name);
	                if (resp.responseType == 0) {
	                    _this.notifyListener(resp);
	                }
	                return resp;
	            }, reject);
	        });
	    };
	    AjaxSender.prototype.notifyListener = function (resp) {
	        var _this = this;
	        if (this._listener != undefined) {
	            setTimeout(function () { return _this._listener.notify(resp); }, 5);
	        }
	    };
	    return AjaxSender;
	}());
	exports.AjaxSender = AjaxSender;


/***/ },
/* 3 */
/***/ function(module, exports) {

	"use strict";
	var CommandResponse = (function () {
	    function CommandResponse(data, commandName) {
	        this.responseType = data.responseType;
	        this.payload = data.payload;
	        this.commandId = data.commandId;
	        if (commandName) {
	            CommandResponse.commandNames[this.commandId] = commandName;
	            this.commandName = commandName;
	        }
	        else {
	            this.commandName = CommandResponse.commandNames[this.commandId];
	        }
	        if (!this.commandName) {
	            throw "The command name cannot be resolved for the commandId " + this.commandId;
	        }
	    }
	    CommandResponse.commandNames = {};
	    return CommandResponse;
	}());
	exports.CommandResponse = CommandResponse;


/***/ },
/* 4 */
/***/ function(module, exports, __webpack_require__) {

	"use strict";
	var __extends = (this && this.__extends) || function (d, b) {
	    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
	    function __() { this.constructor = d; }
	    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
	};
	var ResponseListener_1 = __webpack_require__(5);
	var HttpListener = (function (_super) {
	    __extends(HttpListener, _super);
	    function HttpListener() {
	        _super.call(this);
	    }
	    return HttpListener;
	}(ResponseListener_1.ResponseListener));
	exports.HttpListener = HttpListener;


/***/ },
/* 5 */
/***/ function(module, exports) {

	"use strict";
	var ResponseListener = (function () {
	    function ResponseListener() {
	        this.callbackId = "";
	        this._listeners = {};
	    }
	    ResponseListener.prototype.notify = function (response) {
	        for (var commandName in this._listeners) {
	            var idListeners = this._listeners[commandName][response.commandId];
	            var listeners = this._listeners[commandName][commandName];
	            this.executeCallbacks(idListeners, response);
	            this.executeCallbacks(listeners, response);
	            //unregister callbacks for the commandId
	            delete this._listeners[commandName][response.commandId];
	        }
	    };
	    ResponseListener.prototype.on = function (commandName, commandIdOrCallback, callback) {
	        var commandId = typeof (commandIdOrCallback) == 'string' ? commandIdOrCallback : commandName;
	        callback = callback || commandIdOrCallback;
	        if (this._listeners[commandName] == undefined) {
	            this._listeners[commandName] = {};
	            this._listeners[commandName][commandId] = [];
	        }
	        this._listeners[commandName][commandId].push(callback);
	    };
	    ResponseListener.prototype.off = function (commandName, commandIdOrCallback, callback) {
	        var commandId = typeof (commandIdOrCallback) == 'string' ? commandIdOrCallback : commandName;
	        callback = callback || commandIdOrCallback;
	        var listeners = this._listeners[commandName][commandId];
	        var i = listeners.indexOf(callback);
	        listeners.splice(i, 1);
	    };
	    ResponseListener.prototype.executeCallbacks = function (callbacks, response) {
	        if (callbacks) {
	            callbacks.forEach(function (cb) { return cb(response); });
	        }
	    };
	    return ResponseListener;
	}());
	exports.ResponseListener = ResponseListener;


/***/ },
/* 6 */
/***/ function(module, exports) {

	/// <reference path="../../typings/tsd.d.ts" />
	"use strict";
	var CommandEmitter = (function () {
	    function CommandEmitter(uriBase, commandRequestSender, hubListener) {
	        this.uriBase = uriBase;
	        this._sender = commandRequestSender;
	        this._listener = hubListener;
	    }
	    CommandEmitter.prototype.emit = function (command) {
	        console.info('Sending Command : ' + command.name);
	        var url = this.uriBase + '/' + command.name + '?c=' + (this._listener ? this._listener.callbackId : '');
	        var self = this;
	        return new Promise(function (resolve, reject) {
	            self._sender.send(url, command).then(function (resp) {
	                self._listener.on(command.name, resp.commandId, function (data) {
	                    resolve(data);
	                });
	            });
	        });
	    };
	    return CommandEmitter;
	}());
	exports.CommandEmitter = CommandEmitter;


/***/ },
/* 7 */
/***/ function(module, exports, __webpack_require__) {

	"use strict";
	var __extends = (this && this.__extends) || function (d, b) {
	    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
	    function __() { this.constructor = d; }
	    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
	};
	/// <reference path="../../../typings/tsd.d.ts" />
	var Abstraction_1 = __webpack_require__(3);
	var ResponseListener_1 = __webpack_require__(5);
	var SignalRListener = (function (_super) {
	    __extends(SignalRListener, _super);
	    function SignalRListener(_hubConnection, hubName) {
	        var _this = this;
	        _super.call(this);
	        this._hubConnection = _hubConnection;
	        this._hubConnection.proxies[hubName].client.ReceiveCommandResponse = function (data) { return _this.notify(new Abstraction_1.CommandResponse(data)); };
	    }
	    return SignalRListener;
	}(ResponseListener_1.ResponseListener));
	exports.SignalRListener = SignalRListener;


/***/ },
/* 8 */
/***/ function(module, exports, __webpack_require__) {

	"use strict";
	var HttpListener_1 = __webpack_require__(4);
	var HubListener = (function () {
	    function HubListener(_http, _ws) {
	        this._http = _http;
	        this._ws = _ws;
	        if (!this._http) {
	            this._http = new HttpListener_1.HttpListener();
	        }
	    }
	    Object.defineProperty(HubListener.prototype, "callbackId", {
	        get: function () {
	            return this._ws ? this._ws.callbackId : "";
	        },
	        enumerable: true,
	        configurable: true
	    });
	    HubListener.prototype.on = function (commandName, commandIdOrCallback, callback) {
	        this._http.on(commandName, commandIdOrCallback, callback);
	        if (this._ws) {
	            this._ws.on(commandName, commandIdOrCallback, callback);
	        }
	    };
	    HubListener.prototype.off = function (commandName, commandIdOrCallback, callback) {
	        this._http.off(commandName, commandIdOrCallback, callback);
	        if (this._ws) {
	            this._ws.off(commandName, commandIdOrCallback, callback);
	        }
	    };
	    return HubListener;
	}());
	exports.HubListener = HubListener;


/***/ },
/* 9 */
/***/ function(module, exports, __webpack_require__) {

	/// <reference path="../../typings/tsd.d.ts" />
	"use strict";
	var Command_1 = __webpack_require__(1);
	var CommandEmitter_1 = __webpack_require__(6);
	var AjaxSender_1 = __webpack_require__(2);
	var SignalRListener_1 = __webpack_require__(7);
	var _emitters = new Array();
	exports.findEmitter = function (uriBase) {
	    return _emitters.filter(function (t) { return t.uriBase === uriBase; })[0];
	};
	exports.sendCommand = function (uriBase, name, properties) {
	    var cmd = new Command_1.Command(name, properties);
	    var emitter = exports.findEmitter(uriBase);
	    if (emitter == null) {
	        var listener = null;
	        if ($ && $.connection) {
	            listener = new SignalRListener_1.SignalRListener($.connection.hub, 'crs' + uriBase);
	        }
	        emitter = new CommandEmitter_1.CommandEmitter(uriBase, new AjaxSender_1.AjaxSender(), listener);
	        _emitters.push(emitter);
	    }
	    return emitter.emit(cmd);
	};


/***/ }
/******/ ]);
//# sourceMappingURL=ck-command.js.map
window.CK = window.CK || {};
window.CK.CRS = CRS;