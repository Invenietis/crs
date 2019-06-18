import { ResponseType } from './ResponseType';

export interface CommandResponse<T> {
    commandId: string;
    responseType: ResponseType;
    payload: T
}

