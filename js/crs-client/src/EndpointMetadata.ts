import { CommandResponse } from  './CommandResponse';

export interface EndpointCommandMetadata {
    commandType: string;
    route: {
        fullPath: string,
        prefix: string,
        commandName: string
    };
    traits: string;
    descriptions: string;
    parameters: Array<{
        parameterName: string,
        parameterType:string,
        isAmbientValue: boolean
    }>
}

export interface EndpointMetadata {
    version: number;
    ambientValues?: {[ambientValue: string]: any };
    callerIdPropertyName: string;
    commands?: {[commandName: string]: EndpointCommandMetadata};
}

export interface EndpointMetadataResponse extends CommandResponse<EndpointMetadata> {
}
