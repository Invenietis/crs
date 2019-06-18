import { AxiosInstance } from "axios";
import { MetadataService } from "./MetadataService";
import { readCommandName } from "./Command";
import { AmbientValuesProvider } from './AmbientValuesProvider';
import { CommandResponse } from "./CommandResponse";
import { ResponseReceiver } from "./ResponseReceiver";
import { ResponseType } from "./ResponseType";

function serializeQueryParams(obj: any) {
    const str = [];
    for (let p in obj)
        if (obj.hasOwnProperty(p)) {
            str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
        }

    return str.join("&");
}

export class CommandEmitter {
    public constructor(
        private readonly crsEndpoint: string,
        private readonly axiosInstance: AxiosInstance,
        private readonly metadataService: MetadataService,
        private readonly ambientValuesProvider: AmbientValuesProvider,
        private readonly responseReceivers: ResponseReceiver[]
    ) {}

    private findCallerId(): string {
        // Find a callerId from the ResponseReceivers. A date-based caller ID will be generated otherwise.
        let callerId: string | undefined = undefined;
        for(let i = 0; i < this.responseReceivers.length; i++) {
            const cr = this.responseReceivers[i];
            if(cr.getCallerId !== undefined) {
                callerId = cr.getCallerId();
            }
            if(callerId) {
                break;
            }
        }

        if(callerId) {
            return callerId;
        }
        return new Date().getTime().toString();
    }
    
    public sendCommand<TResult>(command: any): Promise<TResult> {
        const commandName = readCommandName(command);
        const commandPayload = this.ambientValuesProvider.merge(command);

        console.debug(`CRS: Sending command - CommandName: ${commandName}`);

        return this.postCommand<TResult>(commandName, commandPayload).then((response) => {
            // If no receivers, just return the raw response
            let p: Promise<CommandResponse<TResult>> = Promise.resolve(response);
            // Loop on all receivers
            for(let i = 0; i < this.responseReceivers.length; i++) {
                const receiver = this.responseReceivers[i];
                p = p.then(x => receiver.processCommandResponse(x));
            }
            return p.then(x => {
                switch(x.responseType) {
                    case ResponseType.Synchronous:
                    case ResponseType.Meta:
                        console.debug(`CRS: Received command response - CommandName: ${commandName}; CommandId: ${x.commandId}`);
                        return x.payload;
                    case ResponseType.Asynchronous: 
                        throw new Error(`CRS: Received a deferred command response, but no ResponseReceiver handled it. Add an async response receiver (eg. signalr). CommandName: ${commandName}; CommandId: ${x.commandId}`);
                    case ResponseType.ValidationError:
                        console.error(x.payload);
                        throw new Error(`CRS: Received validation error. CommandName: ${commandName}; CommandId: ${x.commandId}`);
                    case ResponseType.InternalErrorResponseType:
                        console.error(x.payload);
                        throw new Error(`CRS: Received internal error. CommandName: ${commandName}; CommandId: ${x.commandId}`);
                    default:
                        throw new Error(`CRS: Unknown ResponseType ${x.responseType}. CommandName: ${commandName}; CommandId: ${x.commandId}`);
                }
            });
        });
    }

    private postCommand<TResult>(commandName: string, commandPayload:any): Promise<CommandResponse<TResult>> {
        const commandUrl = `${this.crsEndpoint}/${commandName}`;
        const callerId = this.findCallerId();

        if(this.metadataService.currentMetadataPromise) {
            return this.metadataService.currentMetadataPromise.then(metadata => {
                const query = {
                    [metadata.callerIdPropertyName]: callerId
                };
    
                return this.axiosInstance
                    .post<CommandResponse<TResult>>(`${commandUrl}?${serializeQueryParams(query)}`, commandPayload)
                    .then(resp => resp.data);
            });
        } else {
            throw new Error('CRS has not been initialized.');
        }
    }
}
