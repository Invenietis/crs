import { AxiosInstance, CancelTokenSource, CancelToken } from "axios";
import axios from "axios";
import { EndpointMetadataResponse, EndpointMetadata } from "./EndpointMetadata";
import { AmbientValuesProvider } from "./AmbientValuesProvider";
import { MetadataOptions, MetadataPath } from "./MetadataOptions";

export class MetadataService {
    private cts?: CancelTokenSource;
    public currentMetadataPromise?: Promise<EndpointMetadata>;

    constructor(
        private readonly crsEndpoint: string,
        private readonly axios: AxiosInstance,
        private readonly options: MetadataOptions,
        private readonly ambientValuesProvider: AmbientValuesProvider
        ) {
    }

    public reloadMetadata() {
        // Cancel previous metadata request
        // (This will fail any pending commands attached to the existing promise
        if(this.cts !== undefined) {
            this.cts.cancel();
        }

        // Fetch metadata and replace local promise
        this.cts = axios.CancelToken.source();
        this.currentMetadataPromise = this.fetchMetadata(this.cts.token).then( (r) => {
            if(r.payload === undefined) {
                throw new Error('CRS: Metadata response didn\'t have a payload property');
            }
            this.ambientValuesProvider.setValues(r.payload.ambientValues);
            console.debug('CRS: Loaded metadata');
            return r.payload;
        } );

        return this.currentMetadataPromise;
    }

    private fetchMetadata(cancelToken: CancelToken): Promise<EndpointMetadataResponse> {
        const url = `${this.crsEndpoint}/${MetadataPath}`;
        console.debug(`CRS: Fetching medatata from ${url}`);
        return this.axios.post(url, this.options, {
            cancelToken: cancelToken,
        }).then(resp => resp.data);
    }
}