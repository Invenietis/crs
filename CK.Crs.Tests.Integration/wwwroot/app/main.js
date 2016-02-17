System.register(['./app', 'angular2/platform/browser', 'angular2/core', 'ck-command/command', 'ck-qrs/core', 'ck-qrs/angular2'], function(exports_1) {
    var app_1, browser_1, core_1, command_1, core_2, angular2_1;
    var httpListener, hub, emitter, registry;
    return {
        setters:[
            function (app_1_1) {
                app_1 = app_1_1;
            },
            function (browser_1_1) {
                browser_1 = browser_1_1;
            },
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (command_1_1) {
                command_1 = command_1_1;
            },
            function (core_2_1) {
                core_2 = core_2_1;
            },
            function (angular2_1_1) {
                angular2_1 = angular2_1_1;
            }],
        execute: function() {
            httpListener = new command_1.HttpListener();
            hub = new command_1.HubListener(httpListener);
            emitter = new command_1.CommandEmitter('/c/admin', new command_1.AjaxSender(httpListener), hub);
            registry = new core_2.RestRegistry(new angular2_1.AngularRestDataReader(core_1.Injector));
            registry.configure({
                user: {
                    url: '/api/user/:id',
                    keys: ['id']
                },
                userList: {
                    url: '/api/user',
                    policies: [
                        new core_2.CrsPolicy(['addUser'], hub)
                    ]
                }
            });
            browser_1.bootstrap(app_1.AppComponent, [
                core_1.provide(command_1.CommandEmitter, { useValue: emitter }),
                core_1.provide(core_2.DataStore, { useValue: new core_2.DataStore(registry) })
            ]);
        }
    }
});
//# sourceMappingURL=main.js.map