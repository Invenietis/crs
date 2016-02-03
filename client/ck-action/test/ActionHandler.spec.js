System.register(['../core', './executors'], function(exports_1) {
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
        }
    }
});
