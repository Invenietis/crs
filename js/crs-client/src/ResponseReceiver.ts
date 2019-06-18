import { CommandResponse } from "./CommandResponse";

export interface ResponseReceiver {
    getCallerId?(): string;
    initialize?(): Promise<void>;
    processCommandResponse<TResult>(r: CommandResponse<TResult>): Promise<CommandResponse<TResult>>;
}
