/// <reference path="../Scripts/jquery.d.ts"/>
/// <reference path="../Scripts/tsUnit.ts"/>
/// <amd-dependency path="../Scripts"  />
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var tsUnit = require('../Scripts/tsUnit');
var Commands = require('../CommandSender');
var CommandSenderTests = (function (_super) {
    __extends(CommandSenderTests, _super);
    function CommandSenderTests() {
        _super.apply(this, arguments);
        this.target = new Commands.CommandSender({
            listen: function (commandId, callback) {
                return null;
            }
        });
    }
    CommandSenderTests.prototype.sendACommandShouldTriggerAnXhrRequestToTheServer = function () {
        this.isTrue(true);
    };
    return CommandSenderTests;
})(tsUnit.TestClass);
exports.CommandSenderTests = CommandSenderTests;
