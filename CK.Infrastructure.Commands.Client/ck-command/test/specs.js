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

	module.exports = __webpack_require__(1);


/***/ },
/* 1 */
/***/ function(module, exports, __webpack_require__) {

	/// <reference path="../typings/tsd.d.ts" />
	var CommandEmitter_1 = __webpack_require__(2);
	var Command_1 = __webpack_require__(3);
	describe("Command Emitter Send Tests", function () {
	    var date = new Date();
	    var commandSender = new CommandEmitter_1.default('/c', {
	        send: function (url, command) {
	            return new Promise(function (resolve, reject) {
	                // Simulate async request
	                setTimeout(function () {
	                    resolve({
	                        commandId: '1234',
	                        payload: '3712',
	                        responseType: 1 // Deferred 
	                    });
	                }, 1000);
	            });
	        }
	    }, {
	        callbackId: '3712',
	        listen: function (commandId, callbackId) {
	            return new Promise(function (resolve, reject) {
	                // Simulate async request
	                setTimeout(function () {
	                    var response = {
	                        payload: {
	                            effectiveDate: date
	                        },
	                        commandId: '1234',
	                        responseType: 0
	                    };
	                    resolve(response);
	                }, 1000);
	            });
	        }
	    });
	    it("Send a command should trigger an Xhr request to the server", function (done) {
	        var command = new Command_1.default('TransferAmount', {
	            sourceAccountId: '7A8125D3-2BF9-45DE-A258-CE0D3C17892D',
	            destinationAccountId: '37EC9EA1-2A13-4A4D-B55E-6C844D822DAC',
	            amount: '500'
	        });
	        var promise = commandSender.emit(command);
	        promise.then(function (response) {
	            expect(response).toBeDefined();
	            expect(response.payload).toBeDefined();
	            expect(response.payload.effectiveDate).toBe(date);
	            expect(response.commandId).toBe('1234');
	            expect(response.responseType).toBe(0);
	            done();
	        });
	    });
	});


/***/ },
/* 2 */
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
	Object.defineProperty(exports, "__esModule", { value: true });
	exports.default = CommandEmitter;


/***/ },
/* 3 */
/***/ function(module, exports) {

	var Command = (function () {
	    function Command(name, properties) {
	        this.name = name;
	        properties = this.properties;
	    }
	    return Command;
	})();
	Object.defineProperty(exports, "__esModule", { value: true });
	exports.default = Command;


/***/ }
/******/ ]);