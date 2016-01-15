import { Command } from 'ck-command/command';

export function ActionHandler<TFunction extends Function>(Target: TFunction, actionName: string): TFunction {
    (<any>Target)._ = '';
    
    return Target;
}

export interface IActionHandler {
    handle(command: Command) : void;
}