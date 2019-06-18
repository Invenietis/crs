import { Command, CrsEndpoint, CrsEndpointConfiguration } from '@signature/crs-client';
import { SignalrResponseReceiver } from '@signature/crs-client-signalr';

@Command("MyDeferredCommand")
export class MyDeferredCommand {
}

const endpointConfig: CrsEndpointConfiguration = {
    url: ' http://localhost:5000/api/crs',
    responseReceivers: [
        new SignalrResponseReceiver(
            ' http://localhost:5000/hubs/crs'
        )
    ]
};

const endpoint = new CrsEndpoint(endpointConfig);

async function initializeCrs() {
    // Get metadata (and connect to SignalR with crs-client-signalr)
    let metadata = await endpoint.initialize();

    // Create command
    const command = new MyDeferredCommand();

    // Send command
    const commandResult = await endpoint.send<string>(command);

    // If server metadata change for some reason (eg. new ambient values after re-authentication),
    // you can reload the command metadata.
    metadata = await endpoint.reloadMetadata();

    // If you send any commands using endpoint.send(), CRS will wait until metadata is available
    // before actually sending it.
}

initializeCrs();
