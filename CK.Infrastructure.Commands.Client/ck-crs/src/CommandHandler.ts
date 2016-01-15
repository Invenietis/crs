import { Command } from 'ck-command/command';

export function CommandHandler<TFunction extends Function>(Target: TFunction, commandName: string): TFunction {
    (<any>Target)._ = '';
    
    return Target;
}

export interface ICommandHandler {
    handle(command: Command) : void;
}