import { Command, CrsEndpoint, CrsEndpointConfiguration } from '@signature/crs-client';
import { SignalrResponseReceiver } from '@signature/crs-client-signalr';

declare global {
    interface Window {
        initializeCrs: () => void;
        reloadMetadata: () => void;
        sendCrsCommand: (commandName: string) => void;
    }
}

@Command("MyDeferredCommand")
export class MyDeferredCommand {
}

@Command("MySynchronousCommand")
export class MySynchronousCommand {
}

const endpointConfig: CrsEndpointConfiguration = {
    url: ' http://localhost:5000/api/crs',
    responseReceivers: [
        new SignalrResponseReceiver(
            'http://localhost:5000/hubs/crs'
        )
    ]
};

const buttonContainer = <HTMLElement>document.getElementById('button-container');
const metadataContainer = <HTMLElement>document.getElementById('pre-metadata-info');
const endpoint = new CrsEndpoint(endpointConfig);

window.initializeCrs = function() {
    endpoint.initialize().then(function (metadata) {
        metadataContainer.innerHTML = JSON.stringify(metadata, undefined, 4);
    }).catch(function(err) {
        alert(err);
    });
}
window.sendCrsCommand = function(commandName) {
    let command: any;
    switch(commandName) {
        case 'MySynchronousCommand': command = new MySynchronousCommand(); break;
        case 'MyDeferredCommand': command = new MyDeferredCommand(); break;
        default: throw new Error(`Not suppported: ${commandName}`);
    }

    endpoint.send<any>(command).then( (result) => {
        alert(result);
    }).catch(function(err) {
        alert(err);
    });
}

window.reloadMetadata = function() {
    endpoint.reloadMetadata().then(function (metadata) {
        metadataContainer.innerHTML = JSON.stringify(metadata, undefined, 4);
    }).catch(function(err) {
        alert(err);
    });
}
