import { CommandResponse } from  './CommandResponse';
import { ResponseType } from './ResponseType';

export interface IResponseFormatter {
    parseResponse<T>(responseData: any): CommandResponse<T>;
}

export function camelCasify(o: any): any {
    const objectType = typeof o;
    const isObject = objectType === 'function' || objectType === 'object' && !!o;
    if(isObject) {
        const newObj: any = {};
        for (var p in o) {
            const value = camelCasify(o[p]);
            const newPropertyName = p.substring(0,1).toLowerCase()+p.substring(1);

            newObj[newPropertyName] = value;
        }
        return newObj;
    }
    return o;
}

export class DefaultResponseFormatter implements IResponseFormatter {
    parseResponse<T>(responseData: any): CommandResponse<T> {
        return responseData;
    }
}

export class PascalCaseResponseFormatter implements IResponseFormatter {
    constructor(
        private readonly transformMeta: boolean = true,
        private readonly transformPayload: boolean = false,
        ) {
        }

    parseResponse<T>(responseData: any): CommandResponse<T> {
        let response: CommandResponse<T> = {
            commandId: responseData.CommandId,
            payload: responseData.Payload,
            responseType: responseData.ResponseType,
        }

        if(
            response.responseType === ResponseType.Meta
            && this.transformMeta
            ) {
            response.payload = camelCasify(response.payload);
        } else if(
            response.responseType !== ResponseType.Meta
            && this.transformPayload
            && response.payload
            ) {
            response.payload = camelCasify(response.payload);
        }

        return response;
    }
}

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
