import { AxiosInstance } from "axios";
import { MetadataService } from "./MetadataService";
import { readCommandName } from "./Command";
import { AmbientValuesProvider } from './AmbientValuesProvider';
import { CommandResponse } from "./CommandResponse";
import { ResponseReceiver } from "./ResponseReceiver";
import { ResponseType } from "./ResponseType";
import { EndpointCommandMetadata, IResponseFormatter } from './EndpointMetadata';
import { buildCrsCallErrorMsg, CrsCallError } from "./CrsCallError";

function serializeQueryParams(obj: any) {
    const str = [];
    for (let p in obj)
        if (obj.hasOwnProperty(p)) {
            str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
        }

    return str.join("&");
}

export class CommandEmitter {
    private readonly defaultCallerId: string;

    public constructor(
        private readonly crsEndpoint: string,
        private readonly axiosInstance: AxiosInstance,
        private readonly metadataService: MetadataService,
        private readonly ambientValuesProvider: AmbientValuesProvider,
        private readonly responseReceivers: ResponseReceiver[],
        private readonly responseFormatter: IResponseFormatter
    ) {
        this.defaultCallerId = Math.random().toString(36).substring(2, 15) + Math.random().toString(36).substring(2, 15);
    }

    private findCallerId(commandName: string, commandMetadata: EndpointCommandMetadata | undefined): Promise<string> {
        // Find a callerId from the ResponseReceivers. The default caller ID will be used otherwise.
        let callerIdPromise: Promise<string> = Promise.resolve(this.defaultCallerId);
        for (let i = 0; i < this.responseReceivers.length; i++) {
            const cr = this.responseReceivers[i];
            if (cr.getCallerId !== undefined) {
                let previousPromise = callerIdPromise;
                callerIdPromise = cr.getCallerId(commandName, commandMetadata).then((x) => {
                    if (x === undefined) { return previousPromise; }
                    return x;
                });
            }
        }
        return callerIdPromise;
    }

    public sendCommand<TResult>(command: any): Promise<TResult> {
        const commandName = readCommandName(command);
        const commandPayload = this.ambientValuesProvider.merge(command);
        console.debug(`CRS: Preparing command - CommandName: ${commandName}`);

        return this.postCommand<TResult>(commandName, commandPayload).then((response) => {
            // If no receivers, just return the raw response
            let p: Promise<CommandResponse<TResult>> = Promise.resolve(response);
            // Loop on all receivers
            for (let i = 0; i < this.responseReceivers.length; i++) {
                const receiver = this.responseReceivers[i];
                p = p.then(x => receiver.processCommandResponse(x));
            }
            return p.then(x => {
                switch (x.responseType) {
                    case ResponseType.Synchronous:
                    case ResponseType.Meta:
                        console.debug(`CRS: Received command response - CommandName: ${commandName}; CommandId: ${x.commandId}`);
                        return x.payload;
                    case ResponseType.Asynchronous:
                        throw new Error(`CRS: Received a deferred command response, but no ResponseReceiver handled it. Add an async response receiver (eg. signalr). CommandName: ${commandName}; CommandId: ${x.commandId}`);
                    case ResponseType.ValidationError:
                        console.error(x.payload);
                        throw new Error(`CRS: Received validation error. CommandName: ${commandName}; CommandId: ${x.commandId}; ${x.payload}`);
                    case ResponseType.InternalErrorResponseType:
                        console.error(x.payload);
                        throw new CrsCallError( buildCrsCallErrorMsg(commandName, x.payload), x.payload, commandName, commandPayload );
                    default:
                        throw new Error(`CRS: Unknown ResponseType ${x.responseType}. CommandName: ${commandName}; CommandId: ${x.commandId}`);
                }
            });
        });
    }

    private findCommandMetadata(commandName: string) {

    }

    private postCommand<TResult>(commandName: string, commandPayload: any): Promise<CommandResponse<TResult>> {
        const commandUrl = `${this.crsEndpoint}/${commandName}`;
        if (this.metadataService.currentMetadataPromise) {
            return this.metadataService.currentMetadataPromise.then(metadata => {
                // Find command metadata
                let commandMetadata: EndpointCommandMetadata | undefined = undefined;
                if (metadata.commands) {
                    commandMetadata = metadata.commands[commandName.toLowerCase()];
                }

                return this.findCallerId(commandName, commandMetadata).then((callerId) => {
                    console.debug(`CRS: Sending command - CommandName: ${commandName}; CallerId: ${callerId}`);

                    const query = {
                        [metadata.callerIdPropertyName]: callerId
                    };

                    return this.axiosInstance
                        .post<CommandResponse<TResult>>(`${commandUrl}?${serializeQueryParams(query)}`, commandPayload)
                        .then(resp => this.responseFormatter.parseResponse(resp.data));
                });
            });
        } else {
            throw new Error('CRS has not been initialized.');
        }

    }
}
