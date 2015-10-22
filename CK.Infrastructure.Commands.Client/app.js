define(["require", "exports", 'Scripts/tsUnit', 'Tests/CommandSenderTest'], function (require, exports, tsUnit, CommandTests) {
    var test = new tsUnit.Test(CommandTests);
    console.log(test);
    var result = test.run();
    result.showResults('result');
});
//# sourceMappingURL=app.js.map