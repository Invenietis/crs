import { CommandResponse } from "./CommandResponse";
import { EndpointCommandMetadata } from "./EndpointMetadata";

export interface ResponseReceiver {
    getCallerId?(commandName: string, commandMetadata: EndpointCommandMetadata|undefined): Promise<string|undefined>;
    initialize?(): Promise<void>;
    processCommandResponse<TResult>(r: CommandResponse<TResult>): Promise<CommandResponse<TResult>>;
}
