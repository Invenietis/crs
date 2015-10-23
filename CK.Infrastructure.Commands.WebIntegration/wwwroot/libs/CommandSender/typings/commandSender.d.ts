interface ICommandResponse {
    /**
    * The identifier of the command. */
    CommandId: string,
    /**
    * The payload of the result. */
    Payload: any,

    ResponseType: Number
}

interface ICommandSender {
    /**
     * @param route  The specific command route name.
     * @param commandBody  The command payload DTO. 
     * @param callback  A callback
     */
    send: (route: string, commandBody: Object) => JQueryPromise<ICommandResponse>;
}

interface ICommandResponseListener {
    listen: (commandId: string, callbackId: string) => JQueryPromise<ICommandResponse>;
}

interface ICommandRequestSender {
    post: (url: string, command: any) => JQueryPromise<any>;
}

interface JQueryStatic {
    commandSender: ICommandSender;
}