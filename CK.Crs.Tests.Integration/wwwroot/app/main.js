System.register(['ck-crs/core', './app', 'angular2/platform/browser', 'angular2/core', 'ck-qrs/core', 'ck-qrs/angular2'], function(exports_1) {
    var core_1, app_1, browser_1, core_2, core_3, angular2_1;
    var httpListener, hub, emitter, registry;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (app_1_1) {
                app_1 = app_1_1;
            },
            function (browser_1_1) {
                browser_1 = browser_1_1;
            },
            function (core_2_1) {
                core_2 = core_2_1;
            },
            function (core_3_1) {
                core_3 = core_3_1;
            },
            function (angular2_1_1) {
                angular2_1 = angular2_1_1;
            }],
        execute: function() {
            httpListener = new core_1.HttpListener();
            hub = new core_1.HubListener(httpListener);
            emitter = new core_1.CommandEmitter('/c/admin', new core_1.AjaxSender(httpListener), hub);
            registry = new core_3.RestRegistry(new angular2_1.AngularRestDataReader(core_2.Injector));
            registry.configure({
                user: {
                    url: '/api/user/:id',
                    keys: ['id']
                },
                userList: {
                    url: '/api/user',
                    policies: [
                        new core_3.CrsPolicy(['addUser'], hub)
                    ]
                }
            });
            browser_1.bootstrap(app_1.AppComponent, [
                core_2.provide(core_1.CommandEmitter, { useValue: emitter }),
                core_2.provide(core_3.DataStore, { useValue: new core_3.DataStore(registry) })
            ]);
        }
    }
});
//# sourceMappingURL=main.js.map