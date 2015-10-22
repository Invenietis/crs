import tsUnit = require('Scripts/tsUnit');
import CommandTests = require('Tests/CommandSenderTest');

var test = new tsUnit.Test(CommandTests);
console.log(test);
var result = test.run();
result.showResults('result');   