System.register(['../core', './handlers'], function(exports_1) {
    "use strict";
    var Ck, test;
    return {
        setters:[
            function (Ck_1) {
                Ck = Ck_1;
            },
            function (test_1) {
                test = test_1;
            }],
        execute: function() {
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
        }
    }
});
