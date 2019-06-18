export const CommandMetadataSym = Symbol('CrsCommandMetadata');

export interface CommandObjectMetadata {
    name: string;
    events: string[];
}

/**
 * The command decorator let you bind a command class with a command name.
 * You can also provides a list of events on wich the emitter will subscirbe on 
 * @param name The command name
 * @param events Any events that the command will trigger during its execution.  
 */
export function Command(name: string, ...events: string[]) {
    return function(target: any) {
        target[CommandMetadataSym] = {name, events};
        return target;
    }
}

export function readMetadata(command: Object): CommandObjectMetadata {
    const meta: CommandObjectMetadata = (<any>command.constructor)[CommandMetadataSym];
    if (!meta) {
        throw new Error('Could not read the command metadata. The decorator is probably missing.');
    }

    return meta;
}

export function readCommandName(command: Object): string {
    const meta = readMetadata(command);
    return meta.name;
}
