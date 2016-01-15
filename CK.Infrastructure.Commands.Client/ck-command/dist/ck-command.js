var CkCommands =
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

	var Command = (function () {
	    function Command(name, properties) {
	        this.name = name;
	        properties = this.properties;
	    }
	    return Command;
	})();
	exports.Command = Command;


/***/ },
/* 3 */
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
/* 4 */
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
/* 5 */
/***/ function(module, exports) {

	

/***/ }
/******/ ]);