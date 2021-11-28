import axios from 'axios';
import { EndpointMetadata, DefaultResponseFormatter } from './EndpointMetadata';
import { MetadataService } from './MetadataService';
import { CommandEmitter } from './CommandEmitter';
import { AmbientValuesProvider } from './AmbientValuesProvider';
import { CrsEndpointConfiguration } from './CrsEndpointConfiguration';
import { ResponseReceiver } from './ResponseReceiver';
import { defaultMetadataOptions } from './MetadataOptions';
import { version } from "./CrsEndpoint.version";

export class CrsEndpoint {
    private readonly config: CrsEndpointConfiguration;
    private readonly metadataService: MetadataService;
    private readonly commandEmitter: CommandEmitter;
    private readonly ambientValuesProvider: AmbientValuesProvider;
    private readonly responseReceivers: ResponseReceiver[];

    public constructor(config: CrsEndpointConfiguration) {
        this.config = { ...config };

        // Ensure valid configuration
        if (!this.config.url) {
            throw new Error('CrsEndpoint configuration does not have a valid url property');
        }

        if (this.config.axiosInstance === undefined) {
            this.config.axiosInstance = axios;
        }
        if (this.config.responseReceivers === undefined) {
            this.config.responseReceivers = [];
        }
        if (this.config.metadataOptions === undefined) {
            this.config.metadataOptions = defaultMetadataOptions;
        }
        if (this.config.responseFormatter === undefined) {
            this.config.responseFormatter = new DefaultResponseFormatter();
        }

        this.responseReceivers = this.config.responseReceivers;

        this.ambientValuesProvider = new AmbientValuesProvider();
        this.metadataService = new MetadataService(
            this.config.url,
            this.config.axiosInstance,
            this.config.metadataOptions,
            this.ambientValuesProvider,
            this.config.responseFormatter
        );
        this.commandEmitter = new CommandEmitter(
            this.config.url,
            this.config.axiosInstance,
            this.metadataService,
            this.ambientValuesProvider,
            this.responseReceivers,
            this.config.responseFormatter
        );
    }

    public send<TResult>(command: any): Promise<TResult> {
        return this.commandEmitter.sendCommand<TResult>(command)
    }

    public reloadMetadata(): Promise<EndpointMetadata> {
        if (this.metadataService.currentMetadataPromise) {
            return this.internalReloadMetadata();
        }
        throw new Error('CRS has not been initialized.');
    }

    public getMetadata(): Promise<EndpointMetadata> {
        if (this.metadataService.currentMetadataPromise) {
            return this.metadataService.currentMetadataPromise;
        }
        throw new Error('CRS has not been initialized.');
    }

    public initialize(): Promise<EndpointMetadata> {
        console.debug('CRS: Initializing');
        let metadataPromise = this.internalReloadMetadata();

        const receiverPromises: Promise<any>[] = [metadataPromise];
        for (let i = 0; i < this.responseReceivers.length; i++) {
            const receiver = this.responseReceivers[i];
            if (receiver.initialize) {
                receiverPromises.push(receiver.initialize());
            }
        }

        return Promise.all(receiverPromises).then(_ => {
            console.debug('CRS: Ready');
            return metadataPromise;
        });
    }

    private internalReloadMetadata(): Promise<EndpointMetadata> {
        return this.metadataService.reloadMetadata();
    }

    /** Gets this client version. */
    public static get clientVersion(): string { return version; }

}
