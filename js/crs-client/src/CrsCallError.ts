export class CrsCallError extends Error {
    commandName: string;
    payload: any;
    command: any;
    constructor(message: string, payload: any, commandName: string, command: any) {
        super(message);

        if (Error.captureStackTrace) {
            Error.captureStackTrace(this, CrsCallError);
        }

        this.name = 'CrsCallError';
        this.commandName = commandName;
        this.command = command;
        this.payload = payload;
    }
}

export function buildCrsCallErrorMsg(commandName: string, responsePayload: any): string {
    let msg: string = `While calling ${commandName}: `;
    if(typeof responsePayload === 'string') {
        const lines = responsePayload.split('\n');
        if(lines.length > 0) {
            msg += lines[0].trim();
        } else {
            msg += responsePayload.toString();
        }
    } else {
        msg += responsePayload.toString();
    }
    return msg;
}
