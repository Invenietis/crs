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
	__webpack_require__(2);
	__webpack_require__(3);
	module.exports = __webpack_require__(4);


/***/ },
/* 1 */
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
/* 2 */
/***/ function(module, exports) {

	/// <reference path="../typings/tsd.d.ts" />
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
/* 3 */
/***/ function(module, exports) {

	/// <reference path="../typings/tsd.d.ts" />
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
/* 4 */
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