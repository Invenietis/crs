import {AppComponent} from './app';
import {bootstrap}    from 'angular2/platform/browser';
import {provide, Injector} from 'angular2/core';
import {CommandEmitter, AjaxSender, HubListener, HttpListener} from 'ck-command/command';
import { DataStore, DataCache, Model, ModelRegistry, RestModelConfig, CrsPolicy, RestRegistry } from 'ck-qrs/core';
import { AngularRestDataReader } from 'ck-qrs/angular2';

var httpListener = new HttpListener();
var hub = new HubListener(httpListener);
var emitter = new CommandEmitter('/c/admin', new AjaxSender(httpListener), hub);

var registry = new RestRegistry(new AngularRestDataReader(Injector));
registry.configure({
    user: {
        url: '/api/user/:id',
        keys: ['id']
    },
    userList: { 
        url: '/api/user',
        policies: [
            new CrsPolicy(['addUser'], hub)
        ]
    }
}); 

bootstrap(AppComponent, [
    provide(CommandEmitter, { useValue: emitter }),
    provide(DataStore, { useValue: new DataStore(registry)})
]);